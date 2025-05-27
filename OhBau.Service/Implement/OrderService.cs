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
using Microsoft.Identity.Client;
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
                    TotalPrice = getCart.TotalPrice,
                    OrderCode = RandomCodeUtil.GenerateRandomCode(6)
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

        public async Task<BaseResponse<Paginate<GetOrders>>> GetAllOrders( int pageNumber, int pageSize)
        {
            var parameters = new ListParameters<Order>(pageNumber, pageSize);

            var cache = _orderCacheInvalidator.GetCacheKeyForList(parameters);
            if (_cache.TryGetValue(cache, out Paginate<GetOrders> GetAllOrders))
            {
                return new BaseResponse<Paginate<GetOrders>>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get orders success(cache)",
                    data = GetAllOrders
                };
            }

            var getOrders = await _unitOfWork.GetRepository<Order>().GetPagingListAsync( include: i => i.Include(a => a.Account),
                page: pageNumber, size: pageSize);

            var mapItems = getOrders.Items.Select(c => new GetOrders
            {
                Id = c.Id,
                TotalPrice = c.TotalPrice,
                CreatedDate = c.CreatedDate,
                PaymentStatus = c.PaymentStatus,
                Email = c.Account.Email,
                Phone = c.Account.Phone
            }).ToList();

            var pagedResponse = new Paginate<GetOrders>
            {
                Items = mapItems,
                Page = pageNumber,
                Size = pageSize,
                Total = mapItems.Count
            };

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };

            _cache.Set(cache, pagedResponse, options);

            return new BaseResponse<Paginate<GetOrders>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Get orders success",
                data = pagedResponse
            };
        }

        public async Task<BaseResponse<Paginate<GetOrderDetails>>> GetOrderDetails(Guid accountId, Guid orderId, int pageNumber, int pageSize)
        {
            var parameters = new ListParameters<GetOrderDetails>(pageNumber, pageSize);
            parameters.AddFilter("accountId", accountId);
            parameters.AddFilter("orderId", orderId);

            var cache = _orderDetailCacheInvalidator.GetCacheKeyForList(parameters);
            if (_cache.TryGetValue(cache, out Paginate<GetOrderDetails> GetOrderDetails))
            {
                return new BaseResponse<Paginate<GetOrderDetails>>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get order details success(cache)",
                    data = GetOrderDetails
                };
            }

            var checkAuthorize = await _unitOfWork.GetRepository<OrderDetail>().GetPagingListAsync(
                predicate: x => x.Order.AccountId == accountId && x.OrderId == orderId,
                include: i => i.Include(c => c.Course)
                               .ThenInclude(c => c.Category)
                               .Include(c => c.Order)
                );

            if (checkAuthorize == null)
            {
                return new BaseResponse<Paginate<GetOrderDetails>>
                {
                    status = StatusCodes.Status403Forbidden.ToString(),
                    message = "You do not have the right to view orders that do not belong to you",
                    data = null
                };
            }

            var mapItems = checkAuthorize.Items.Select(c => new GetOrderDetails
            {
                Id = c.Id,
                CourseName = c.Course.Name,
                CategoryName = c.Course.Category.Name,
                Duration = c.Course.Duration,
                CourseRating = c.Course.Rating,
                Price = c.Course.Price
            }).ToList();

            var pagedResponse = new Paginate<GetOrderDetails>
            {
                Items = mapItems,
                Page = pageNumber,
                Size = pageSize,
                Total = mapItems.Count
            };

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };

            _cache.Set(cache,pagedResponse,options);

            return new BaseResponse<Paginate<GetOrderDetails>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Get order details success",
                data = pagedResponse
            };

        }

        public async Task<BaseResponse<Paginate<GetOrders>>> GetOrders(Guid accountId, int pageNumber, int pageSize)
        {
            var parameters = new ListParameters<Order>(pageNumber, pageSize);
            parameters.AddFilter("accountId", accountId);

            var cache = _orderCacheInvalidator.GetCacheKeyForList(parameters);
            if (_cache.TryGetValue(cache, out Paginate<GetOrders> GetOrders))
            {
                return new BaseResponse<Paginate<GetOrders>>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get orders success(cache)",
                    data = GetOrders
                };
            }

            var getOrders = await _unitOfWork.GetRepository<Order>().GetPagingListAsync(predicate: x => x.AccountId == accountId, include: i => i.Include(a => a.Account),
                page: pageNumber, size: pageSize);

            var mapItems = getOrders.Items.Select(c => new GetOrders
            {
                Id = c.Id,
                TotalPrice = c.TotalPrice,
                CreatedDate = c.CreatedDate,
                PaymentStatus = c.PaymentStatus,
                Email = c.Account.Email,
                Phone = c.Account.Phone
            }).ToList();

            var pagedResponse = new Paginate<GetOrders>
            {
                Items = mapItems,
                Page = pageNumber,
                Size = pageSize,
                Total = mapItems.Count
            };

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };

            _cache.Set(cache,pagedResponse, options);

            return new BaseResponse<Paginate<GetOrders>>
            {
                 status = StatusCodes.Status200OK.ToString(),
                 message = "Get orders success",
                 data = pagedResponse
            };

        }
    }
}
