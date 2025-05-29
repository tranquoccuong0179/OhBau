using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
using OhBau.Model.Payload.Response.FavoriteCourse;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class FavoriteCourseService : BaseService<FavoriteCourseService>, IFavoriteCourseService
    {
        private readonly IMemoryCache _cache;
        private readonly GenericCacheInvalidator<FavoriteCourses> _favoriteCourseCache;
        public FavoriteCourseService(IUnitOfWork<OhBauContext> unitOfWork, 
            ILogger<FavoriteCourseService> logger, IMapper mapper, 
            IHttpContextAccessor httpContextAccessor, IMemoryCache cache, GenericCacheInvalidator<FavoriteCourses> favoriteCouseCache) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _cache = cache;
            _favoriteCourseCache = favoriteCouseCache;
        }

        public async Task<BaseResponse<string>> AddDeleteFavoriteCourse(Guid accountId, Guid courseId)
        {
            try
            {
                var checkAlready = await _unitOfWork.GetRepository<FavoriteCourses>().SingleOrDefaultAsync(
                    predicate: x => x.CourseId == courseId && x.AccountId == accountId);
                if(checkAlready != null)
                {
                    _unitOfWork.GetRepository<FavoriteCourses>().DeleteAsync(checkAlready);
                    await _unitOfWork.CommitAsync();
                    
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status200OK.ToString(),
                        message = "Remove favorite course success",
                        data = null
                    };
                }

                var addNewFavoriteCourse = new FavoriteCourses{

                    AccountId = accountId,
                    CourseId = courseId
                };

                await _unitOfWork.GetRepository<FavoriteCourses>().InsertAsync(addNewFavoriteCourse);
                await _unitOfWork.CommitAsync();

                _favoriteCourseCache.InvalidateEntityList();
                _favoriteCourseCache.InvalidateEntity(courseId);

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Add Favorite Course Success",
                    data = null
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<BaseResponse<Paginate<FavoriteCoursesResponse>>> GetFavoriteCourse(int pageNumber, int pageSize,Guid accountId, string? courseName, string? category)
        {
            Expression<Func<FavoriteCourses, bool>> predicate = null;

            if (!string.IsNullOrEmpty(courseName))
            {
                predicate = x => x.AccountId == accountId && x.Course.Name.Contains(courseName) && x.AccountId == accountId;
            }

            if (!string.IsNullOrEmpty(category))
            {
                predicate = x => x.AccountId == accountId && x.Course.Category.Name.Contains(category);
            }

            if(!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(courseName))
            {
                predicate = x => x.AccountId == accountId && x.Course.Name.Contains(courseName) && x.Course.Category.Name.Contains(category);
            }

            var listParameter = new ListParameters<FavoriteCoursesResponse>(pageNumber, pageSize);
            listParameter.AddFilter("courseName",courseName);
            listParameter.AddFilter("category", category);

            var cache = _favoriteCourseCache.GetCacheKeyForList(listParameter);
            if (_cache.TryGetValue(cache,out Paginate<FavoriteCoursesResponse> GetFavoriteCourse))
            {
                return new BaseResponse<Paginate<FavoriteCoursesResponse>>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get favorite course success(cache)",
                    data = GetFavoriteCourse
                };
            }
            var getFavoriteCourse = await _unitOfWork.GetRepository<FavoriteCourses>().GetPagingListAsync(
                predicate: predicate,
                include: i => i.Include(x => x.Course)
                               .ThenInclude(x => x.Category),
                               page: pageNumber,
                               size: pageSize
                               );

            var mapItem = getFavoriteCourse.Items.Select(x => new FavoriteCoursesResponse
            {
                CourseId = x.Course.Id,
                Name = x.Course.Name,
                Category = x.Course.Category.Name,
                Duration = x.Course.Duration
            }).ToList();

            var pagedResponse = new Paginate<FavoriteCoursesResponse>
            {
                Items = mapItem,
                Page = pageNumber,
                Size = pageSize,
                Total = mapItem.Count
            };

            var option = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };

            _cache.Set(cache,pagedResponse,option);
            _favoriteCourseCache.AddToListCacheKeys(cache);

            return new BaseResponse<Paginate<FavoriteCoursesResponse>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Get favorite course success",
                data = pagedResponse
            };

        }
    }
}
