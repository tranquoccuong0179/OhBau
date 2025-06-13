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
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Blog;
using OhBau.Model.Payload.Response.Cart;
using OhBau.Model.Payload.Response.Order;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class CartrService : BaseService<CartrService>, ICartService
    {
        private readonly IMemoryCache _cache;
        private readonly GenericCacheInvalidator<Cart> _cartCacheInvalidator;
        private readonly GenericCacheInvalidator<CartItems> _cartItemsInvalidator;

        public CartrService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<CartrService> logger, 
            IMapper mapper, 
            IHttpContextAccessor httpContextAccessor,
            GenericCacheInvalidator<Cart> cartCacheInvalidator, IMemoryCache cache,
            GenericCacheInvalidator<CartItems> cartItemsInvalidator) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _cartCacheInvalidator = cartCacheInvalidator;
            _cache = cache;
            _cartItemsInvalidator = cartItemsInvalidator;
        }

        public async Task<BaseResponse<string>> AddProductToCart(Guid productId,int quantity, Guid accountId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {

                var getCartByAccount = await _unitOfWork.GetRepository<Cart>().GetByConditionAsync(x => x.AccountId == accountId);

                var checkAlready = await _unitOfWork.GetRepository<CartItems>().GetByConditionAsync(
                    x => x.CartId == getCartByAccount.Id && x.ProductId == productId);

                var getProduct = await _unitOfWork.GetRepository<Product>().GetByConditionAsync(x => x.Id == productId);


                if (checkAlready != null)
                {
                    if (getProduct.Quantity < quantity + checkAlready.Quantity)
                    {
                        return new BaseResponse<string>
                        {
                            status = StatusCodes.Status400BadRequest.ToString(),
                            message = "The quantity of products in stock is not enough",
                            data = null
                        };
                    }

                    getCartByAccount.TotalPrice -= checkAlready.TotalPrice;

                    checkAlready.Quantity += quantity;
                    checkAlready.TotalPrice = checkAlready.UnitPrice * checkAlready.Quantity;

                    getCartByAccount.TotalPrice += checkAlready.TotalPrice;

                    _unitOfWork.GetRepository<CartItems>().UpdateAsync(checkAlready);
                    _unitOfWork.GetRepository<Cart>().UpdateAsync(getCartByAccount);

                    await _unitOfWork.CommitAsync();
                    await _unitOfWork.CommitTransactionAsync();
                    _cartCacheInvalidator.InvalidateEntityList();
                    _cartItemsInvalidator.InvalidateEntityList();

                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status200OK.ToString(),
                        message = "Add product success",
                        data = null
                    };
                }


                    if (getProduct.Quantity < quantity)
                {
                    _cartCacheInvalidator.InvalidateEntityList();
                    _cartItemsInvalidator.InvalidateEntityList();
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status400BadRequest.ToString(),
                        message = "The quantity of products in stock is not enough",
                        data = null
                    };
                }

                if (getCartByAccount == null)
                {
                    _cartCacheInvalidator.InvalidateEntityList();
                    _cartItemsInvalidator.InvalidateEntityList();
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status404NotFound.ToString(),
                        message = "Cart not found",
                        data = null
                    };
                }

                var addNewProduct = new CartItems
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    CartId = getCartByAccount.Id,
                    UnitPrice = getProduct.Price,
                    Quantity = quantity,
                    TotalPrice = getProduct.Price * quantity
                };

                await _unitOfWork.GetRepository<CartItems>().InsertAsync(addNewProduct);
                await _unitOfWork.CommitAsync();

                getCartByAccount.TotalPrice = getCartByAccount.TotalPrice + (addNewProduct.UnitPrice * quantity);

                _unitOfWork.GetRepository<Cart>().UpdateAsync(getCartByAccount);
                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                _cartCacheInvalidator.InvalidateEntityList();
                _cartCacheInvalidator.InvalidateEntity(addNewProduct.Id);
                _cartItemsInvalidator.InvalidateEntityList();

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Add product to order success",
                    data = null
                };
            }
            catch (Exception ex) {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception(ex.ToString());
            }
        }

        public async Task<BaseResponse<Paginate<GetCartByAccount>>> GetCartItemByAccount(Guid accountId, int pageNumber, int pageSize)
        {
            var listParameter = new ListParameters<Cart>(pageNumber, pageSize);
            listParameter.AddFilter("accountId", accountId);

            var cacheKey = _cartCacheInvalidator.GetCacheKeyForList(listParameter);

            if (_cache.TryGetValue(cacheKey, out Paginate<GetCartByAccount> cachedResult))
            {
                return new BaseResponse<Paginate<GetCartByAccount>>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get cart success(cache)",
                    data = cachedResult
                };
            }

            var getCartItems = await _unitOfWork.GetRepository<CartItems>().GetPagingListAsync(predicate: a => a.Cart.AccountId == accountId,
                                                                                                                 include: i =>
                                                                                                                 i.Include(c => c.Products)
                                                                                                                 .Include(o => o.Cart)
                                                                                                                 .ThenInclude(a => a.Account),
                                                                                                                 page: pageNumber,
                                                                                                                 size: pageSize
                                                                                                                 );
                var mapItem = getCartItems.Items.Select(o => new GetCartItem
                {
                       ItemId = o.Id,
                       Name = o.Products.Name,
                       ImageUrl = o.Products.Image,
                       Description = o.Products.Description,
                       Color = o.Products.Color,
                       Size = o.Products.Size,
                       UnitPrice = o.Products.Price,
                }).ToList();

                var result = new GetCartByAccount
                {
                    CartId = getCartItems.Items.Select(x => x.Cart.Id).FirstOrDefault(),
                    cartItem = mapItem,
                    TotalPrice = getCartItems.Items.Select(x => x.Cart.TotalPrice).FirstOrDefault(),
                    TotalItem = mapItem.Count
                };

                var pagedResposne = new Paginate<GetCartByAccount>
                {
                    Items = new List<GetCartByAccount> { result },
                    Page = pageNumber,
                    Size = pageSize,
                    Total = mapItem.Count,
                    TotalPages = getCartItems.TotalPages
                };

                var cacheOption = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
                };

                 _cache.Set(cacheKey, pagedResposne, cacheOption);
                _cartCacheInvalidator.AddToListCacheKeys(cacheKey);

                return new BaseResponse<Paginate<GetCartByAccount>>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get orders success",
                    data = pagedResposne
                };
                                                                                                  
            }
        
        public async Task<BaseResponse<Paginate<Model.Payload.Response.Cart.GetCartDetailResponse>>> GetCartDetails(Guid accountId, int pageNumber, int pageSize)
        {

            var listParameter = new ListParameters<Cart>(pageNumber, pageSize);
            listParameter.AddFilter("accountId", accountId);

            var cacheKey = _cartCacheInvalidator.GetCacheKeyForList(listParameter);
            if (_cache.TryGetValue(cacheKey, out Paginate<Model.Payload.Response.Cart.GetCartDetailResponse> getCartDetail))
            {
                return new BaseResponse<Paginate<GetCartDetailResponse>>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get cart items success(cache)",
                    data = getCartDetail
                };
            }
            var getCartDetails = await _unitOfWork.GetRepository<CartItems>().GetPagingListAsync(
                predicate: x => x.Cart.AccountId == accountId,
                include: i => i.Include(x => x.Products)
                               .Include(c => c.Cart).ThenInclude(a => a.Account),
                page: pageNumber,
                size: pageSize
            );

            var mapItems = getCartDetails.Items.Select(c => new CartItemDetail
            {
                ItemId = c.Id,
                Name = c.Products.Name,
                ImageUrl = c.Products.Image,
                Description = c.Products.Description,
                Brand = c.Products.Brand,
                Color = c.Products.Color,
                Size = c.Products.Size,
                AgeRange = c.Products.AgeRange,
                Price = c.UnitPrice,
                Quantity = c.Quantity,
                TotalPrice = c.TotalPrice
            }).ToList();

            var result = new GetCartDetailResponse
            {
                CartItems = mapItems,
                TotalPrice = getCartDetails.Items.Select(x => x.Cart.TotalPrice).FirstOrDefault(),
            };

            var pagedResposne = new Paginate<GetCartDetailResponse>
            {
                Items = new List<GetCartDetailResponse> { result },
                Page = pageNumber,
                Size = pageSize,
                Total = mapItems.Count,
                TotalPages = getCartDetails.TotalPages
            };

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };

            _cache.Set(cacheKey, pagedResposne, options);

            return new BaseResponse<Paginate<GetCartDetailResponse>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Get cart detail success",
                data = pagedResposne
            };
        }

        public async Task<BaseResponse<string>> DeleteCartItem(Guid itemId)
        {
            try
            {
                var checkDelete = await _unitOfWork.GetRepository<CartItems>().GetByConditionAsync(condition => condition.Id == itemId);
                if (checkDelete == null)
                {
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status404NotFound.ToString(),
                        message = "Cart item not found",
                        data = null
                    };
                }

                var getCart = await _unitOfWork.GetRepository<Cart>().SingleOrDefaultAsync(predicate: x => x.Id == checkDelete.CartId);
                getCart.TotalPrice = getCart.TotalPrice - checkDelete.TotalPrice;
                _unitOfWork.GetRepository<Cart>().UpdateAsync(getCart);
                _unitOfWork.GetRepository<CartItems>().DeleteAsync(checkDelete);
                await _unitOfWork.CommitAsync();

                _cartCacheInvalidator.InvalidateEntityList();
                _cartCacheInvalidator.InvalidateEntity(itemId);
                _cartItemsInvalidator.InvalidateEntityList();
                _cartItemsInvalidator.InvalidateEntity(itemId);

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Delte cart item success",
                    data = null
                };
            }
            catch (Exception ex) { 
                
                throw new NotImplementedException(); 
            }
        }

        public async Task<BaseResponse<string>> UpdateQuantityItem(Guid itemId, int quantity)
        {
            try
            {
                if (quantity < 1)
                {
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status400BadRequest.ToString(),
                        message = "Quantity must not be less than 1",
                        data = null
                    };
                }
                var getCartItem = await _unitOfWork.GetRepository<CartItems>()
                                                   .GetByConditionAsync(x => x.Id == itemId);

                if (getCartItem == null)
                {
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status404NotFound.ToString(),
                        message = "Cart item not found",
                        data = null
                    };
                }

                // Lấy cart để cập nhật tổng giá
                var getCart = await _unitOfWork.GetRepository<Cart>()
                                               .GetByConditionAsync(x => x.Id == getCartItem.CartId);

                // Trừ tổng giá cũ
                getCart.TotalPrice -= getCartItem.TotalPrice;

                // Cập nhật lại thông tin cart item
                getCartItem.Quantity = quantity;
                getCartItem.TotalPrice = getCartItem.UnitPrice * quantity;

                // Cộng tổng giá mới
                getCart.TotalPrice += getCartItem.TotalPrice;

                // Cập nhật và lưu
                 _unitOfWork.GetRepository<CartItems>().UpdateAsync(getCartItem);
                 _unitOfWork.GetRepository<Cart>().UpdateAsync(getCart);

                await _unitOfWork.CommitAsync();
                _cartCacheInvalidator.InvalidateEntityList();
                _cartItemsInvalidator.InvalidateEntityList();

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Update quantity success",
                    data = null
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

    }
}
