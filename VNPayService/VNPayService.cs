using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Enum;
using OhBau.Repository.Interface;
using OhBau.Service;
using VNPayService.Config;
using VNPayService.DTO;
using VNPayService.VNPlayPackage;

namespace VNPayService
{
    public class VNPayService :BaseService<VNPayService> ,IVnPayService
    {
        private readonly VNPayConfig _vnPayConfig;
        private readonly GenericCacheInvalidator<MyCourse> _mycourseCacheInvalidator;
        public VNPayService(IUnitOfWork<OhBauContext> unitOfWork, 
            ILogger<VNPayService> logger, IMapper mapper, 
            IHttpContextAccessor httpContextAccessor, VNPayConfig vnPayConfig,
            GenericCacheInvalidator<MyCourse> myCourseCacheInvalidator) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _vnPayConfig = vnPayConfig;
            _mycourseCacheInvalidator = myCourseCacheInvalidator;
        }

        public async Task<string> CreatePayment(CreateOrder request)
        {
            try
            {
          
                var getOrder = await _unitOfWork.GetRepository<Order>().GetByConditionAsync(x => x.Id == request.OrderId);

                if (getOrder == null)
                {
                    return "404";
                }

                var createVnPayOrder = new OrderInfo
                {
                    OrderID = request.OrderId,
                    Amount = (float)getOrder.TotalPrice,
                    CreatedDate = DateTime.Now,
                    Des = request.Des,
                    locale = "VN",
                    Status = 0,
                };
                decimal vnpAmount = (decimal)getOrder.TotalPrice * 100;
                _logger.LogInformation("VNPay request for OrderId {OrderId}. vnp_Amount: {VnpAmount}", request.OrderId, vnpAmount.ToString("F0"));
                var vnPay = new VNPayLibrary();
                vnPay.AddRequestData("vnp_Version", "2.1.0");
                vnPay.AddRequestData("vnp_Command", "pay");
                vnPay.AddRequestData("vnp_TmnCode", _vnPayConfig.TmnCode);
                vnPay.AddRequestData("vnp_Amount", vnpAmount.ToString("F0"));
                vnPay.AddRequestData("vnp_CreateDate", createVnPayOrder.CreatedDate.ToString("yyyyMMddHHmmss"));
                vnPay.AddRequestData("vnp_CurrCode", "VND");
                vnPay.AddRequestData("vnp_IpAddr", _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1");
                vnPay.AddRequestData("vnp_Locale", createVnPayOrder.locale);
                vnPay.AddRequestData("vnp_OrderInfo", createVnPayOrder.Des);
                vnPay.AddRequestData("vnp_OrderType", "other");
                vnPay.AddRequestData("vnp_ReturnUrl", _vnPayConfig.ReturnUrl);
                vnPay.AddRequestData("vnp_TxnRef", createVnPayOrder.OrderID.ToString());

                string paymentUrl = vnPay.CreateRequestUrl(_vnPayConfig.PaymentUrl,_vnPayConfig.SecretKey);
                return paymentUrl;
            }
            catch (Exception ex) { 
            
                throw new Exception(ex.ToString());

            }

        }

        public async Task<string> VnPayReturn()
        {
            var vnPaydata = _httpContextAccessor.HttpContext.Request.Query;
            var vnPay = new VNPayLibrary();
            foreach (var item in vnPaydata.Keys) {
                if (!string.IsNullOrEmpty(item) && item.StartsWith("vnp_"))
                {
                    vnPay.AddResponseData(item, vnPaydata[item]);
                }
            }

            string txnRefStr = vnPay.GetResponseData("vnp_TxnRef");
            Guid orderId = Guid.NewGuid();
            if (!Guid.TryParse(txnRefStr, out orderId))
            {
                return "{\"RspCode\":\"01\",\"Message\":\"Invalid transaction reference\"}";
            }

            string amountStr = vnPay.GetResponseData("vnp_Amount");
            if (string.IsNullOrEmpty(amountStr) || !long.TryParse(amountStr, out long vnp_AmountRaw))
            {
                _logger.LogError("Invalid vnp_Amount format: {AmountStr}", amountStr);
                return "{\"RspCode\":\"01\",\"Message\":\"Invalid amount\"}";
            }
            decimal vnp_Amount = vnp_AmountRaw / 100m;

            string tranIdStr = vnPay.GetResponseData("vnp_TransactionNo");
            long vnpayTranId = 0;
            if (!long.TryParse(tranIdStr, out vnpayTranId))
            {
                return "{\"RspCode\":\"01\",\"Message\":\"Invalid transaction number\"}";
            }

            string vnp_ResponseCode = vnPay.GetResponseData("vnp_ResponseCode");
            string vnp_TransactionStatus = vnPay.GetResponseData("vnp_TransactionStatus");
            string vnp_SecureHash = vnPaydata["vnp_SecureHash"]!;

            bool checkSignature = vnPay.ValidateSignature(vnp_SecureHash, _vnPayConfig.SecretKey);

            if (!checkSignature)    
            {
                return "{\"RspCode\":\"97\",\"Message\":\"Invalid signature\"}";
            }

            var order = await _unitOfWork.GetRepository<Order>().GetByConditionAsync(x => x.Id == orderId);


            if (order == null)
            {
                return "{\"RspCode\":\"01\",\"Message\":\"Order not found\"}";
            }

            _logger.LogInformation("Comparing amounts for orderId {OrderId}. Order TotalPrice: {TotalPrice}, vnp_Amount: {VnpAmount}",
                orderId, order.TotalPrice, vnp_Amount);

            if ((decimal)order.TotalPrice != vnp_Amount)
            {
                _logger.LogError("Amount mismatch for orderId {OrderId}. Order TotalPrice: {TotalPrice}, vnp_Amount: {VnpAmount}",
                    orderId, order.TotalPrice, vnp_Amount);
                return "{\"RspCode\":\"04\",\"Message\":\"Invalid amount\"}";
            }

            if (order.PaymentStatus != PaymentStatusEnum.Pending)
            {
                return "{\"RspCode\":\"02\",\"Message\":\"Order already confirmed\"}";
            }

            if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
            {
                order.PaymentStatus = PaymentStatusEnum.Paid;
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var getAccountByOrder = await _unitOfWork.GetRepository<Account>().GetByConditionAsync(x => x.Id == order.AccountId);
                var getOrderDetailByOrder = await _unitOfWork.GetRepository<OrderDetail>().GetListAsync(predicate: x => x.OrderId == order.Id);

                foreach (var orderDetail in getOrderDetailByOrder)
                {
                    var addNewMyCourse = new MyCourse
                    {
                        AccountId = getAccountByOrder.Id,
                        CourseId = orderDetail.CourseId,
                        CreateAt = DateTime.Now,
                    };

                    await _unitOfWork.GetRepository<MyCourse>().InsertAsync(addNewMyCourse);
                    _unitOfWork.GetRepository<Order>().UpdateAsync(order);
                }

                var getCartByAccount = await _unitOfWork.GetRepository<Cart>().GetByConditionAsync(x => x.AccountId == order.AccountId);
                getCartByAccount.TotalPrice = 0;
                
                var getCartItemByCart = await _unitOfWork.GetRepository<CartItems>().GetListAsync(predicate: x => x.CartId == getCartByAccount.Id);

                _unitOfWork.GetRepository<CartItems>().DeleteRangeAsync(getCartItemByCart);
                _unitOfWork.GetRepository<Cart>().UpdateAsync(getCartByAccount);
                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                _mycourseCacheInvalidator.InvalidateEntityList();
            }
            catch (Exception ex) {
                
              await  _unitOfWork.RollbackTransactionAsync();
                throw new Exception(ex.ToString());
            }

            return "{\"RspCode\":\"00\",\"Message\":\"Confirm Success\"}";
        }

        public async Task<PaymentResponse> ProcessVnPayReturn(Dictionary<string, string> queryParams)
        {
            try
            {
                var vnp_HashSecret = _vnPayConfig?.SecretKey;
                VNPayLibrary vnpay = new VNPayLibrary();

                foreach (var key in queryParams)
                {
                    if (!string.IsNullOrEmpty(key.Key) && key.Key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(key.Key, key.Value);
                    }
                }

                var vnp_SecureHash = queryParams["vnp_SecureHash"];
                var checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

                if (!checkSignature)
                {
                    return new PaymentResponse
                    {
                        IsSuccessful = false,
                        Message = "Chữ ký không hợp lệ."
                    };
                }

                string txnRefStr = vnpay.GetResponseData("vnp_TxnRef");
                if (!Guid.TryParse(txnRefStr, out Guid orderId))
                {
                    return new PaymentResponse
                    {
                        IsSuccessful = false,
                        Message = "Mã tham chiếu giao dịch không hợp lệ."
                    };
                }
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                string bankCode = vnpay.GetResponseData("vnp_BankCode");
                string terminalId = vnpay.GetResponseData("vnp_TmnCode");

                if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                {
                    return new PaymentResponse
                    {
                        IsSuccessful = true,
                        Message = "Giao dịch thành công.",
                        OrderId = orderId,
                        VnpayTransactionId = vnpayTranId,
                        Amount = vnp_Amount,
                        BankCode = bankCode,
                        TerminalId = terminalId
                    };
                }

                return new PaymentResponse
                {
                    IsSuccessful = false,
                    Message = $"Giao dịch thất bại. Mã lỗi: {vnp_ResponseCode}"
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}

