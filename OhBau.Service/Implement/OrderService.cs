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

        public async Task<BaseResponse<CreateOrderResponse>> CreateOrder(Guid accountId, CreateOrderRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var checkOrderAlready = await _unitOfWork.GetRepository<Order>().SingleOrDefaultAsync(
                    include: i => i.Include(x => x.OrderDetails).ThenInclude(p => p.Products),
                    predicate: x => x.AccountId == accountId && x.PaymentStatus == PaymentStatusEnum.Pending
                );

                if (checkOrderAlready != null)
                {
                    var existingProductIds = checkOrderAlready.OrderDetails.Select(x => x.ProductId).ToHashSet();

                    foreach (var newItem in request.Items)
                    {
                        var getCartItem = await _unitOfWork.GetRepository<CartItems>().GetByConditionAsync(x => x.Id == newItem.ItemId);

                        if (getCartItem == null)
                        {
                            continue;
                        }

                        if (existingProductIds.Contains(getCartItem.ProductId))
                        {
                            continue;
                        }

                        var addNewDetailItem = new OrderDetail
                        {
                            Id = Guid.NewGuid(),
                            OrderId = checkOrderAlready.Id,
                            ProductId = getCartItem.ProductId,
                            UnitPrice = getCartItem.UnitPrice,
                            Quantity = getCartItem.Quantity,
                            TotalPrice = getCartItem.UnitPrice * getCartItem.Quantity,
                        };

                        await _unitOfWork.GetRepository<OrderDetail>().InsertAsync(addNewDetailItem);
                        checkOrderAlready.TotalPrice += addNewDetailItem.TotalPrice;
                        _unitOfWork.GetRepository<Order>().UpdateAsync(checkOrderAlready);
                        await _unitOfWork.CommitAsync();
                    }

                    await _unitOfWork.CommitTransactionAsync();

                    _orderCacheInvalidator.InvalidateEntityList();
                    _orderDetailCacheInvalidator.InvalidateEntityList();

                    var response = new CreateOrderResponse
                    {
                        OrderCode = checkOrderAlready.OrderCode,
                        //OrderItems = checkOrderAlready.OrderDetails.Select(x => new Model.Payload.Response.Order.OrderItem
                        //{
                        //    ProductName = x.Products.Name,
                        //    Price = x.Products.Price,
                        //    Quantity = x.Quantity
                        //}).ToList(),
                        TotalPrice = checkOrderAlready.TotalPrice
                    };

                    return new BaseResponse<CreateOrderResponse>
                    {
                        status = StatusCodes.Status200OK.ToString(),
                        message = "Update order success",
                        data = response
                    };
                }

                // Create new order if no pending order exists
                var createNewOrder = new Order
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    CreatedDate = DateTime.Now,
                    OrderCode = RandomCodeUtil.GenerateRandomCode(6),
                    PaymentStatus = PaymentStatusEnum.Pending,
                    TotalPrice = 0
                };

                await _unitOfWork.GetRepository<Order>().InsertAsync(createNewOrder);
                await _unitOfWork.CommitAsync();

                foreach (var items in request.Items)
                {
                    var getCartItems = await _unitOfWork.GetRepository<CartItems>().GetByConditionAsync(
                        predicate: x => x.Id == items.ItemId);

                    if (getCartItems == null)
                    {
                        continue;
                    }

                    var addNewItem = new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        OrderId = createNewOrder.Id,
                        ProductId = getCartItems.ProductId,
                        Quantity = getCartItems.Quantity,
                        UnitPrice = getCartItems.UnitPrice,
                        TotalPrice = getCartItems.UnitPrice * getCartItems.Quantity
                    };

                    createNewOrder.TotalPrice += addNewItem.TotalPrice;

                    await _unitOfWork.GetRepository<OrderDetail>().InsertAsync(addNewItem);
                    _unitOfWork.GetRepository<Order>().UpdateAsync(createNewOrder);
                    await _unitOfWork.CommitAsync();
                }

                await _unitOfWork.CommitTransactionAsync();

                _orderCacheInvalidator.InvalidateEntityList();
                _orderDetailCacheInvalidator.InvalidateEntityList();

                var newResponse = new CreateOrderResponse
                {
                    OrderCode = createNewOrder.OrderCode,
                    TotalPrice = createNewOrder.TotalPrice
                };

                return new BaseResponse<CreateOrderResponse>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Create order success",
                    data = newResponse
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception(ex.ToString());
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
            _orderCacheInvalidator.AddToListCacheKeys(cache);

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
                include: i => i.Include(c => c.Products)
                               .ThenInclude(c => c.ProductCategory)
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
                Name  = c.Products.Name,
                ImageUrl = c.Products.Image,
                Description = c.Products.Description,
                Brand = c.Products.Brand,
                Color = c.Products.Color,
                Size = c.Products.Size,
                AgeRange = c.Products.AgeRange,
                Quantity = c.Quantity,
                Price = c.UnitPrice,
                TotalPrice = c.TotalPrice
            }).ToList();

            var pagedResponse = new Paginate<GetOrderDetails>
            {
                Items = mapItems,
                Page = pageNumber,
                Size = pageSize,
                Total = mapItems.Count()
            };

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };

            _cache.Set(cache,pagedResponse,options);
            _orderDetailCacheInvalidator.AddToListCacheKeys(cache);

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
            _orderCacheInvalidator.AddToListCacheKeys(cache);

            return new BaseResponse<Paginate<GetOrders>>
            {
                 status = StatusCodes.Status200OK.ToString(),
                 message = "Get orders success",
                 data = pagedResponse
            };

        }
    }
}
