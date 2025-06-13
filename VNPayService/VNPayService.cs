﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Enum;
using OhBau.Model.Exception;
using OhBau.Model.Payload.Response;
using OhBau.Model.Utils;
using OhBau.Repository.Interface;
using OhBau.Service;
using VNPayService.Config;
using VNPayService.DTO;
using VNPayService.VNPlayPackage;
#pragma warning disable
namespace VNPayService
{
    public class VNPayService :BaseService<VNPayService> ,IVnPayService
    {
        private readonly VNPayConfig _vnPayConfig;
        private readonly GenericCacheInvalidator<MyCourse> _mycourseCacheInvalidator;
        private readonly GenericCacheInvalidator<Order> _orderCache;
        private readonly GenericCacheInvalidator<Course> _courseCacheInvalidator;
        private readonly GenericCacheInvalidator<Cart> _cartCacheInvalidator;
        private readonly GenericCacheInvalidator<Transaction> _transactionCacheInvalidator;
        private readonly GenericCacheInvalidator<OrderDetail> _orderDetailCacheInvalidator;
        private readonly GenericCacheInvalidator<CartItems> _cartItemsCacheInvalidator;
        public VNPayService(
            IUnitOfWork<OhBauContext> unitOfWork,
            ILogger<VNPayService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            VNPayConfig vnPayConfig,
            GenericCacheInvalidator<MyCourse> myCourseCacheInvalidator,
            GenericCacheInvalidator<Order> orderCache,
            GenericCacheInvalidator<Course> courseCacheInvalidator,
            GenericCacheInvalidator<Cart> cartCacheInvalidator,
            GenericCacheInvalidator<Transaction> transactionCacheInvalidator,
            GenericCacheInvalidator<OrderDetail> orderDetailCache,
            GenericCacheInvalidator<CartItems> cartItemCache
            
        ) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _vnPayConfig = vnPayConfig;
            _mycourseCacheInvalidator = myCourseCacheInvalidator;
            _orderCache = orderCache;
            _courseCacheInvalidator = courseCacheInvalidator;
            _cartCacheInvalidator = cartCacheInvalidator;
            _transactionCacheInvalidator = transactionCacheInvalidator;
            _orderDetailCacheInvalidator = orderDetailCache;
            _cartItemsCacheInvalidator = cartItemCache;
        }

        public async Task<string> CreatePayment(CreateOrder request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
          
                var getOrder = await _unitOfWork.GetRepository<Order>().GetByConditionAsync(x => x.OrderCode == request.OrderCode);

                if (getOrder == null)
                {
                    return "404";
                }

                var createVnPayOrder = new OrderInfo
                {
                    OrderCode = request.OrderCode,
                    Amount = (float)getOrder.TotalPrice,
                    CreatedDate = DateTime.Now,
                    Des = request.Des,
                    locale = "VN",
                    Status = 0,
                    
                };
                decimal vnpAmount = (decimal)getOrder.TotalPrice * 100;
                _logger.LogInformation("VNPay request for OrderId {OrderId}. vnp_Amount: {VnpAmount}", request.OrderCode, vnpAmount.ToString("F0"));
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
                vnPay.AddRequestData("vnp_TxnRef", createVnPayOrder.OrderCode.ToString());

                var checkTransaction = await _unitOfWork.GetRepository<Transaction>().GetByConditionAsync(x => x.OrderId.Equals(getOrder.Id) 
                && x.Status == PaymentStatusEnum.Pending && x.ExpireDate > DateTime.Now);
                if (checkTransaction != null)
                {
                    return checkTransaction.PaymentUrl;
                }

                string paymentUrl = vnPay.CreateRequestUrl(_vnPayConfig.PaymentUrl,_vnPayConfig.SecretKey);
      

                var addTransaction = new Transaction
                {
                    Id = LongIdGeneratorUtil.GenerateUniqueLongId(),
                    CreatedDate = DateTime.Now,
                    ExpireDate = DateTime.Now.AddMinutes(15),
                    PaymentUrl = paymentUrl,
                    Status = PaymentStatusEnum.Pending,
                    Provider = PaymentTypeEnum.VNPay
                };

                if (request.OrderCode.Length == 6)
                {
                    addTransaction.Type = TransactionTypeEnum.Booking;
                    addTransaction.OrderId = getOrder.Id;
                }
                await _unitOfWork.GetRepository<Transaction>().InsertAsync(addTransaction);
                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();
                return paymentUrl;
            }
            catch (Exception ex) {

                await _unitOfWork.RollbackTransactionAsync();
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
            string orderCode = txnRefStr;
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

            var order = await _unitOfWork.GetRepository<Order>().GetByConditionAsync(x => x.OrderCode == orderCode);
            var transaction = await _unitOfWork.GetRepository<Transaction>().GetByConditionAsync(x => x.OrderId == order.Id);


            if (order == null)
            {
                return "{\"RspCode\":\"01\",\"Message\":\"Order not found\"}";
            }

            _logger.LogInformation("Comparing amounts for orderId {OrderId}. Order TotalPrice: {TotalPrice}, vnp_Amount: {VnpAmount}",
                orderCode, order.TotalPrice, vnp_Amount);

            if ((decimal)order.TotalPrice != vnp_Amount)
            {
                _logger.LogError("Amount mismatch for orderId {OrderId}. Order TotalPrice: {TotalPrice}, vnp_Amount: {VnpAmount}",
                    orderCode, order.TotalPrice, vnp_Amount);
                return "{\"RspCode\":\"04\",\"Message\":\"Invalid amount\"}";
            }

            if (order.PaymentStatus != PaymentStatusEnum.Pending)
            {
                return "{\"RspCode\":\"02\",\"Message\":\"Order already confirmed\"}";
            }

            if (vnp_ResponseCode == "24" && vnp_TransactionStatus == "02")
            {
                order.PaymentStatus = PaymentStatusEnum.Cancelled;
                transaction.Status = PaymentStatusEnum.Cancelled;
                _unitOfWork.GetRepository<Order>().UpdateAsync(order);
                _unitOfWork.GetRepository<Transaction>().UpdateAsync(transaction);
                await _unitOfWork.CommitAsync();

                return "{\"RspCode\":\"02\",\"Message\":\"Transaction canceled\"}";
            }
            if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
             {
                double totalPrice = 0;
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    order.PaymentStatus = PaymentStatusEnum.Paid;
                    transaction.Status = PaymentStatusEnum.Paid;

                     _unitOfWork.GetRepository<Order>().UpdateAsync(order);
                     _unitOfWork.GetRepository<Transaction>().UpdateAsync(transaction);
                    await _unitOfWork.CommitAsync();

                    var getOrderDetailByOrderCode = await _unitOfWork.GetRepository<OrderDetail>()
                        .GetListAsync(predicate: x => x.OrderId == order.Id && x.Order.PaymentStatus == PaymentStatusEnum.Paid,
                                      include: x => x.Include(x => x.Order));

                    foreach (var deleteItem in getOrderDetailByOrderCode)
                    {
                        var getCartItem = await _unitOfWork.GetRepository<CartItems>().SingleOrDefaultAsync(
                            predicate: x => x.ProductId == deleteItem.ProductId);

                        if (getCartItem != null)
                        {
                            var getProduct = await _unitOfWork.GetRepository<Product>().GetByConditionAsync(x => x.Id == getCartItem.ProductId);
                            getProduct.Quantity = getProduct.Quantity - getCartItem.Quantity;
                            totalPrice += getCartItem.UnitPrice * getCartItem.Quantity;
                           _unitOfWork.GetRepository<CartItems>().DeleteAsync(getCartItem);
                            _unitOfWork.GetRepository<Product>().UpdateAsync(getProduct);
                        }
                    }

                    var getCart = await _unitOfWork.GetRepository<Cart>().GetByConditionAsync(x => x.AccountId == order.AccountId);
                    if (getCart != null)
                    {
                        getCart.TotalPrice = getCart.TotalPrice - totalPrice;
                        _unitOfWork.GetRepository<Cart>().UpdateAsync(getCart);
                    }
                    await _unitOfWork.CommitAsync();
                    await _unitOfWork.CommitTransactionAsync();
                }

                catch (Exception ex)
                {

                    await _unitOfWork.RollbackTransactionAsync();
                    throw new Exception(ex.ToString());
                }
                return "{\"RspCode\":\"00\",\"Message\":\"Confirm Success\"}";

            }
            else
            {
                throw new Exception();
            }
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

        public async Task<string> CreatePaymentBooking(Guid id)
        {
            Guid? userId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);

            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: a => a.Id.Equals(userId) && a.Active == true);
            if (account == null)
            {
                throw new NotFoundException("Không tìm thấy tài khoản");
            }

            var parent = await _unitOfWork.GetRepository<Parent>().SingleOrDefaultAsync(
                predicate: p => p.AccountId.Equals(userId) && p.Active == true);

            if (parent == null)
            {
                throw new NotFoundException("Không tìm thấy thông tin bố mẹ");
            }

            var booking = await _unitOfWork.GetRepository<Booking>().SingleOrDefaultAsync(
                predicate: b => b.Id.Equals(id) && b.ParentId.Equals(parent.Id));

            if (booking == null)
            {
                throw new NotFoundException("Không tìm thấy thông tin booking hoặc không phải của bạn");
            }

            try
            {
                decimal vnpAmount = (decimal)100000 * 100;
                var vnPay = new VNPayLibrary();
                vnPay.AddRequestData("vnp_Version", "2.1.0");
                vnPay.AddRequestData("vnp_Command", "pay");
                vnPay.AddRequestData("vnp_TmnCode", _vnPayConfig.TmnCode);
                vnPay.AddRequestData("vnp_Amount", vnpAmount.ToString("F0"));
                vnPay.AddRequestData("vnp_CreateDate", TimeUtil.GetCurrentSEATime().ToString("yyyyMMddHHmmss"));
                vnPay.AddRequestData("vnp_CurrCode", "VND");
                vnPay.AddRequestData("vnp_IpAddr", _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1");
                vnPay.AddRequestData("vnp_Locale", "VN");
                vnPay.AddRequestData("vnp_OrderInfo", booking.Id.ToString());
                vnPay.AddRequestData("vnp_OrderType", "other");
                vnPay.AddRequestData("vnp_ReturnUrl", _vnPayConfig.ReturnUrl);
                vnPay.AddRequestData("vnp_TxnRef", booking.Id.ToString());

                string paymentUrl = vnPay.CreateRequestUrl(_vnPayConfig.PaymentUrl, _vnPayConfig.SecretKey);

                var addTransaction = new Transaction
                {
                    Id = LongIdGeneratorUtil.GenerateUniqueLongId(),
                    CreatedDate = DateTime.Now,
                    PaymentUrl = paymentUrl,
                    Status = PaymentStatusEnum.Pending,
                    Provider = PaymentTypeEnum.VNPay,
                    OrderId = booking.Id,
                    Type = TransactionTypeEnum.BuyCourse,
                };

                await _unitOfWork.GetRepository<Transaction>().InsertAsync(addTransaction);
                await _unitOfWork.CommitAsync();
                return paymentUrl;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.ToString());

            }
        }
    }
}

