using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Cart;
using OhBau.Model.Payload.Response.Order;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class CartrService : BaseService<CartrService>, ICartService
    {
        public CartrService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<CartrService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<string>> AddCourseToCart(Guid courseId, Guid accountId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var getCartByAccount = await _unitOfWork.GetRepository<Cart>().GetByConditionAsync(x => x.AccountId == accountId);
                var checkAlready = await _unitOfWork.GetRepository<CartItems>().GetByConditionAsync(x => x.CartId == getCartByAccount.Id && x.CourseId == courseId);
                if (checkAlready != null)
                {
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status208AlreadyReported.ToString(),
                        message = "The course already exists in your order",
                        data = null
                    };
                }

                var getCourse = await _unitOfWork.GetRepository<Course>().GetByConditionAsync(x => x.Id == courseId);
                if (getCourse == null)
                {
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status404NotFound.ToString(),
                        message = "Course not found",
                        data = null
                    };
                }

                var addNewCourse = new CartItems
                {
                    Id = Guid.NewGuid(),
                    CourseId = courseId,
                    CartId = getCartByAccount.Id,
                    UnitPrice = getCourse.Price
                };

                await _unitOfWork.GetRepository<CartItems>().InsertAsync(addNewCourse);
                await _unitOfWork.CommitAsync();

                getCartByAccount.TotalPrice = getCartByAccount.TotalPrice + addNewCourse.UnitPrice;

                _unitOfWork.GetRepository<Cart>().UpdateAsync(getCartByAccount);
                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();
                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Add course to order success",
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
            try
            {
                var getCartItems = await _unitOfWork.GetRepository<CartItems>().GetPagingListAsync(predicate: a => a.Cart.AccountId == accountId,
                                                                                                                 include: i =>
                                                                                                                 i.Include(c => c.Course)
                                                                                                                 .Include(o => o.Cart)
                                                                                                                 .ThenInclude(a => a.Account),
                                                                                                                 page: pageNumber,
                                                                                                                 size: pageSize
                                                                                                                 );
                var mapItem = getCartItems.Items.Select(o => new GetCartItem
                {
                       Name = o.Course.Name,
                       UnitPrice = o.Course.Price,
                }).ToList();

                var result = new GetCartByAccount
                {
                    CardId = getCartItems.Items.Select(x => x.Cart.Id).FirstOrDefault(),
                    cartItem = mapItem,
                    TotalPrice = getCartItems.Items.Select(x => x.Cart.TotalPrice).FirstOrDefault()
                };
                var pagedResposne = new Paginate<GetCartByAccount>
                {
                    Items = new List<GetCartByAccount> { result },
                    Page = pageNumber,
                    Size = pageSize,
                    Total = mapItem.Count,
                    TotalPages = getCartItems.TotalPages
                };

                return new BaseResponse<Paginate<GetCartByAccount>>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get orders success",
                    data = pagedResposne
                };
                                                                                                  
            }
            catch (Exception ex) { 
                
                throw new Exception(ex.ToString());
            }
        }

        public async Task<BaseResponse<Paginate<Model.Payload.Response.Cart.GetCartDetailResponse>>> GetCartDetails(Guid accountId, int pageNumber, int pageSize)
        {
            try
            {
                var getCartDetails = await _unitOfWork.GetRepository<CartItems>().GetPagingListAsync(
                    predicate: x => x.Cart.AccountId == accountId,
                    include: i => i.Include(x => x.Course)
                                   .Include(c => c.Cart).ThenInclude(a => a.Account),
                    page: pageNumber,
                    size: pageSize
                );

                var mapItems = getCartDetails.Items.Select(c => new CartItemDetail
                {
                    DetailId = c.Id,
                    Name = c.Course.Name,
                    Duration = c.Course.Duration,
                    Price = c.Course.Price,
                    Rating = c.Course.Rating
                }).ToList();

                var result = new GetCartDetailResponse
                {
                    CartItems = mapItems,
                    TotalPrice  = getCartDetails.Items.Select(x => x.Cart.TotalPrice).FirstOrDefault(),
                };

                var pagedResposne = new Paginate<GetCartDetailResponse>
                {
                    Items = new List<GetCartDetailResponse> { result },
                    Page = pageNumber,
                    Size = pageSize,
                    Total = mapItems.Count,
                    TotalPages = getCartDetails.TotalPages
                };


                return new BaseResponse<Paginate<GetCartDetailResponse>>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get cart detail success",
                    data = pagedResposne
                };
            }
            catch (Exception ex) {

                throw new NotImplementedException();

            }
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
                getCart.TotalPrice = getCart.TotalPrice - checkDelete.UnitPrice;
                _unitOfWork.GetRepository<Cart>().UpdateAsync(getCart);
                _unitOfWork.GetRepository<CartItems>().DeleteAsync(checkDelete);
                await _unitOfWork.CommitAsync();

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


    }
}
