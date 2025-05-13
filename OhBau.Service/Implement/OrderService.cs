using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Enum;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Order;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Order;
using OhBau.Model.Utils;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class OrderService : BaseService<OrderService>, IOrderSerivce
    {
        private readonly IMemoryCache _cache;
        private readonly GenericCacheInvalidator<Order> _orderCacheInvalidator;
        private readonly GenericCacheInvalidator<OrderDetail> _orderDetailCacheInvalidator;
        public OrderService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<OrderService> logger, 
            IMapper mapper, 
            IHttpContextAccessor httpContextAccessor, 
            GenericCacheInvalidator<Order> orderCacheInvalidator,
            IMemoryCache cache,
            GenericCacheInvalidator<OrderDetail> orderDetailCacheInvalidator) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _orderCacheInvalidator = orderCacheInvalidator;
            _cache = cache;
            _orderDetailCacheInvalidator = orderDetailCacheInvalidator;
        }

        public async Task<BaseResponse<CreateOrderResponse>> CreateOrder(CreateOrderRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var getCart = await _unitOfWork.GetRepository<Cart>().SingleOrDefaultAsync(
                    predicate: x => x.Id == request.CartId,
                    include: q => q.Include(a => a.Account));

                var getCartItem = await _unitOfWork.GetRepository<CartItems>().GetListAsync(
                    predicate: x => x.CartId == request.CartId,
                    include: q => q.Include(ci => ci.Course)); 

                var checkOrderready = await _unitOfWork.GetRepository<Order>().GetByConditionAsync(
                    predicate: x => x.AccountId == getCart.AccountId &&
                                    x.PaymentStatus == PaymentStatusEnum.Pending);

                var orderItems = new List<OrderItem>();

                if (checkOrderready != null)
                {
                    var deleteOrderDetail = await _unitOfWork.GetRepository<OrderDetail>()
                        .GetListAsync(predicate: x => x.OrderId == checkOrderready.Id);

                    _unitOfWork.GetRepository<OrderDetail>().DeleteRangeAsync(deleteOrderDetail);
                    await _unitOfWork.CommitAsync();

                    foreach (var item in getCartItem)
                    {
                        var addNewOrderDetail = new OrderDetail
                        {
                            Id = Guid.NewGuid(),
                            CourseId = item.CourseId,
                            OrderId = checkOrderready.Id,
                            UnitPrice = item.UnitPrice,
                        };

                        await _unitOfWork.GetRepository<OrderDetail>().InsertAsync(addNewOrderDetail);

                        orderItems.Add(new OrderItem
                        {
                            Course = item.Course.Name,
                            Price = item.UnitPrice
                        });
                    }

                    checkOrderready.TotalPrice = getCart.TotalPrice;
                    _unitOfWork.GetRepository<Order>().UpdateAsync(checkOrderready);
                    await _unitOfWork.CommitAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    _orderCacheInvalidator.InvalidateEntityList();
                    _orderCacheInvalidator.InvalidateEntity(checkOrderready.Id);

                    return new BaseResponse<CreateOrderResponse>
                    {
                        status = StatusCodes.Status200OK.ToString(),
                        message = "Order updated with new cart items.",
                        data = new CreateOrderResponse
                        {
                            OrderId = checkOrderready.Id,
                            TotalPrice = checkOrderready.TotalPrice,
                            OrderItems = orderItems
                        }
                    };
                }

                var newOrderId = Guid.NewGuid();
                var createNewOrder = new Order
                {
                    Id = newOrderId,
                    AccountId = getCart.AccountId,
                    CreatedDate = DateTime.Now,
                    PaymentStatus = PaymentStatusEnum.Pending,
                    TotalPrice = getCart.TotalPrice
                };

                await _unitOfWork.GetRepository<Order>().InsertAsync(createNewOrder);

                foreach (var newItem in getCartItem)
                {
                    var createNewOrderDetail = new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        CourseId = newItem.CourseId,
                        OrderId = newOrderId,
                        UnitPrice = newItem.UnitPrice
                    };

                    await _unitOfWork.GetRepository<OrderDetail>().InsertAsync(createNewOrderDetail);

                    orderItems.Add(new OrderItem
                    {
                        Course = newItem.Course.Name,
                        Price = newItem.UnitPrice
                    });
                }

                _orderCacheInvalidator.InvalidateEntityList();
                _orderCacheInvalidator.InvalidateEntity(createNewOrder.Id);

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new BaseResponse<CreateOrderResponse>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Order created successfully.",
                    data = new CreateOrderResponse
                    {
                        OrderId = newOrderId,
                        TotalPrice = getCart.TotalPrice,
                        OrderItems = orderItems
                    }
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new BaseResponse<CreateOrderResponse>
                {
                    status = StatusCodes.Status500InternalServerError.ToString(),
                    message = ex.ToString(),
                    data = null
                };
            }
        }

    }
}
