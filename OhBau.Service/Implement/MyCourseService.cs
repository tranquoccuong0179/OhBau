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
using OhBau.Model.Payload.Response.MyCourse;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class MyCourseService : BaseService<MyCourseService>, IMyCourseService
    {
        private readonly IMemoryCache _cache;
        private readonly GenericCacheInvalidator<MyCourse> _myCourseCacheInvalidator;
        public MyCourseService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<MyCourseService> logger, 
            IMapper mapper, IHttpContextAccessor httpContextAccessor,
            IMemoryCache cache,
            GenericCacheInvalidator<MyCourse> myCourseCacheInvalidator) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _cache = cache;
            _myCourseCacheInvalidator = myCourseCacheInvalidator;
        }

        public async Task<BaseResponse<Paginate<MyCoursesResponse>>> MyCourses(Guid accountId, int pageNumber, int pageSize, string? courseName)
        {
            var listParameter = new ListParameters<MyCourse>(pageNumber, pageSize);
            listParameter.AddFilter("courseName", courseName);
            listParameter.AddFilter("accountId", accountId);

            var cache = _myCourseCacheInvalidator.GetCacheKeyForList(listParameter);
            if (_cache.TryGetValue(cache, out Paginate<MyCoursesResponse> MyCourses))
            {
                return new BaseResponse<Paginate<MyCoursesResponse>>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get my courses success(cache)",
                    data = MyCourses
                };
            }

            Expression<Func<MyCourse,bool>> predicate = x => x.AccountId == accountId;
            
            if (!string.IsNullOrEmpty(courseName))
            {
                predicate = x => x.AccountId == accountId && x.Course.Name.Contains(courseName);
            }

            var getCoursesByAccount = await _unitOfWork.GetRepository<MyCourse>().GetPagingListAsync(
                predicate:predicate,
                include: x => x.Include(c => c.Course).ThenInclude(c => c.Category),
                page: pageNumber,
                size: pageSize
                );

            var mapItems = getCoursesByAccount.Items.Select(mc => new MyCoursesResponse
            {
                Id = mc.CourseId,
                Name = mc.Course.Name,
                Duration = mc.Course.Duration,
                Rating = mc.Course.Rating,
                Category = mc.Course.Category.Name
            }).ToList();

            var pagedResponse = new Paginate<MyCoursesResponse>
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

            _cache.Set(cache, pagedResponse ,options);
            _myCourseCacheInvalidator.AddToListCacheKeys(cache);

            return new BaseResponse<Paginate<MyCoursesResponse>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Get my course success",
                data = pagedResponse
            };
        }

        public async Task<BaseResponse<string>> ReceiveCourse(Guid accountId, Guid courseId)
        {
            try
            {
               var checkReceive = await _unitOfWork.GetRepository<MyCourse>().GetByConditionAsync(x => x.AccountId == accountId && x.CourseId == courseId);
                if (checkReceive != null)
                {
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status208AlreadyReported.ToString(),
                        message = "You have already received this course",
                        data = null
                    };
                }

                var addMyCourse = new MyCourse
                {
                    AccountId = accountId,
                    CourseId = courseId,
                    CreateAt = DateTime.Now
                };

                await _unitOfWork.GetRepository<MyCourse>().InsertAsync(addMyCourse);
                await _unitOfWork.CommitAsync();

                _myCourseCacheInvalidator.InvalidateEntityList();
                _myCourseCacheInvalidator.InvalidateEntity(courseId);


                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Receive course succss",
                    data = null
                };
            }
            catch (Exception ex) { 
                
                throw new Exception(ex.ToString(), ex);
            }
        }
    }
}
