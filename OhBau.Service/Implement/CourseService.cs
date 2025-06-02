using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Course;
using OhBau.Model.Payload.Request.Topic;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Course;
using OhBau.Model.Payload.Response.Topic;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;
using OhBau.Service.Redis;

namespace OhBau.Service.Implement
{
    public class CourseService : BaseService<CourseService>, ICourseService
    {
        private readonly IMemoryCache _cache;
        private readonly GenericCacheInvalidator<Course> _courseCacheInvalidator;
        //private readonly IRedisService _redisService;
        private readonly GenericCacheInvalidator<Topic> _topicCacheValidator;
        public CourseService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<CourseService> logger,
            IMapper mapper, IHttpContextAccessor httpContextAccessor, 
            GenericCacheInvalidator<Course> courseCacheInvalidator,
            IMemoryCache cache,
            GenericCacheInvalidator<Topic> topicCacheValidator) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _courseCacheInvalidator = courseCacheInvalidator;
            _cache = cache;
            _topicCacheValidator = topicCacheValidator;
        }

        public async Task<BaseResponse<CreateCourseResponse>> CreateCourse(CreateCourseRequest request)
        {
            try
            {
                var createNewCourse = new Course
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Active = request.IsActive,
                    Price = request.Price,
                    CreateAt = request.CreateAt,
                    Duration = request.Duration,
                    UpdateAt = null,
                    DeleteAt = null,
                    CategoryId  = request.CategoryId
                };

                var reponse = new CreateCourseResponse
                {
                    Id = createNewCourse.Id,
                };

                await _unitOfWork.GetRepository<Course>().InsertAsync(createNewCourse);
                await _unitOfWork.CommitAsync();

                _courseCacheInvalidator.InvalidateEntityList();
                _courseCacheInvalidator.InvalidateEntity(createNewCourse.Id);

                //await _redisService.RemoveByPatternAsync("Courses:*");

                return new BaseResponse<CreateCourseResponse>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Create course success",
                    data = reponse
                };
            }

            catch (Exception ex) {

                throw new Exception(ex.ToString());
            }
        }

        public async Task<BaseResponse<string>> DeleteCourse(Guid courseId)
        {
            try
            {
                var checkDelele = await _unitOfWork.GetRepository<Course>().GetByConditionAsync(x => x.Id == courseId);
                if (checkDelele == null)
                {
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status404NotFound.ToString(),
                        message = "Course not found",
                        data = null
                    };
                }

                checkDelele.Active = false;
                checkDelele.DeleteAt = DateTime.Now;
                 _unitOfWork.GetRepository<Course>().UpdateAsync(checkDelele);
                await _unitOfWork.CommitAsync();

                _courseCacheInvalidator.InvalidateEntityList();
                _courseCacheInvalidator.InvalidateEntity(courseId);

                //await _redisService.RemoveByPatternAsync("Courses:*");

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Delete Success",
                    data = null
                };
            }
            catch (Exception ex) { 
            
               throw new Exception(ex.ToString(), ex);  
            }
        }

        public async Task<BaseResponse<Paginate<GetCoursesResponse>>> GetCoursesWithFilterOrSearch(int pageSize, int pageNumber, string? categoryName, string? search)
        {
            var listParameter = new ListParameters<Course>(pageNumber, pageSize);
            listParameter.AddFilter("Category", categoryName);
            listParameter.AddFilter("Search", search);

            var cacheKey = _courseCacheInvalidator.GetCacheKeyForList(listParameter);
            if (_cache.TryGetValue(cacheKey, out Paginate<GetCoursesResponse> GetCourses))
            {
                return new BaseResponse<Paginate<GetCoursesResponse>>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get course success(cache)",
                    data = GetCourses
                };
            }

            //var cacheKey = $"Courses:Page{pageNumber}_Size{pageSize}_Category:{categoryName}_Search:{search}";
            //var checkCacheData = await _redisService.GetAsync<Paginate<GetCoursesResponse>>(cacheKey);
            //if (checkCacheData != null)
            //{
            //    return new BaseResponse<Paginate<GetCoursesResponse>>
            //    {
            //        status = StatusCodes.Status200OK.ToString(),
            //        message = "Get course success (cache)",
            //        data = checkCacheData
            //    };
            //}

            Expression<Func<Course, bool>> predicate = null;
            if (!string.IsNullOrEmpty(categoryName))
            {
                predicate = c => c.Category.Name.Contains(categoryName);
            }
            if (!string.IsNullOrEmpty(search))
            {
                predicate = c => c.Name.Contains(search);
            }

            if (!string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(categoryName))
            {
                predicate = c => c.Name.Contains(search) && c.Category.Name.Contains(categoryName);
            }

            var getCourses = await _unitOfWork.GetRepository<Course>().GetPagingListAsync(predicate : predicate,
                                                                                          include: i => i.Include(c => c.Category),
                                                                                          page:pageNumber,
                                                                                          size:pageSize);
     
            var mappedItem = getCourses.Items.Select(c => new GetCoursesResponse
            {
                Id = c.Id,
                Name = c.Name,
                Duration = c.Duration,
                Rating = c.Rating,
                Price = c.Price,
                Active = c.Active,
                CreateAt = c.CreateAt,
                UpdateAt = c.UpdateAt,
                DeleteAt = c.DeleteAt,
                Category = c.Category.Name
            }).ToList();

            var pagedReponse = new Paginate<GetCoursesResponse>
            {
                Items = mappedItem,
                Page  = pageNumber,
                Size = pageSize,
                Total = mappedItem.Count,
                TotalPages = getCourses.TotalPages
            };

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };

            _cache.Set(cacheKey, pagedReponse, options);
            _courseCacheInvalidator.AddToListCacheKeys(cacheKey);
            //await _redisService.SetAsync(cacheKey, pagedReponse, TimeSpan.FromMinutes(30));


            return new BaseResponse<Paginate<GetCoursesResponse>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Get course success",
                data = pagedReponse
            };

        }

        public async Task<BaseResponse<string>> UpdateCourse(Guid courseId, UpdateCourse request)
        {
            try
            {
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

                getCourse.Name = request.Name ?? getCourse.Name;
                getCourse.Price = request.Price != 0 ? request.Price : getCourse.Duration;
                getCourse.CategoryId = request.CategoryId != null ? request.CategoryId : getCourse.CategoryId;
                getCourse.Active = request.Active != null ? request.Active : getCourse.Active;
                getCourse.CreateAt = getCourse.CreateAt;
                getCourse.UpdateAt = getCourse.UpdateAt;
                getCourse.DeleteAt = getCourse.DeleteAt;

                _unitOfWork.GetRepository<Course>().UpdateAsync(getCourse);
                await _unitOfWork.CommitAsync();

                _courseCacheInvalidator.InvalidateEntityList();
                _courseCacheInvalidator.InvalidateEntity(courseId);

                //await _redisService.RemoveByPatternAsync("Courses:*");

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Update course success",
                    data = null
                };
            }
            catch (Exception ex) {

                throw new Exception(ex.ToString());
            }

        }

        public async Task<BaseResponse<CreateTopicResponse>> CreateTopic(CreateTopicRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var createTopic = new Topic
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Description = request.Description,
                    CourseId = request.CourseId,
                    IsDelete = false,
                    Duration = request.Duration
                };

                await _unitOfWork.GetRepository<Topic>().InsertAsync(createTopic);
                await _unitOfWork.CommitAsync();

                var getTopics = await _unitOfWork.GetRepository<Topic>().GetListAsync(predicate: x => x.CourseId == request.CourseId);
                
                var updateCourse = await _unitOfWork.GetRepository<Course>().GetByConditionAsync(x => x.Id == request.CourseId);
                updateCourse.Duration = getTopics.Sum(x => x.Duration);

                _unitOfWork.GetRepository<Course>().UpdateAsync(updateCourse);
                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                _topicCacheValidator.InvalidateEntityList();
                _topicCacheValidator.InvalidateEntity(createTopic.Id);
                _courseCacheInvalidator.InvalidateEntityList();
                var response = new CreateTopicResponse
                {
                    TopicId  = createTopic.Id
                };

                return new BaseResponse<CreateTopicResponse>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Create topic success",
                    data = response
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception(ex.ToString());
            }
        }

        public async Task<BaseResponse<Paginate<GetTopics>>> GetTopics(Guid courseId, string? courseName, int pageNumber, int pageSize)
        {
            var parameters = new ListParameters<Topic>(pageNumber, pageSize);
            parameters.AddFilter("courseId", courseId);
            parameters.AddFilter("courseName", courseName);

            var cacheKey = _topicCacheValidator.GetCacheKeyForList(parameters);
            if (_cache.TryGetValue(cacheKey, out Paginate<GetTopics> GetTopics))
            {
                return new BaseResponse<Paginate<GetTopics>>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get topics succes(cache)",
                    data = GetTopics
                };
            }

            Expression<Func<Topic, bool>> predicate = x => x.CourseId == courseId && x.IsDelete == false;

            if (!string.IsNullOrEmpty(courseName))
            {
                predicate = x => x.CourseId == courseId && x.Course.Name.Contains(courseName) && x.IsDelete == false;
            }

            var getTopics = await _unitOfWork.GetRepository<Topic>().GetPagingListAsync(predicate: predicate,
                include: i => i.Include(c => c.Course),
                page: pageNumber,
                size: pageSize);

            var mapItems = getTopics.Items.Select(x => new GetTopics
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Duration = x.Duration,
                IsDelete = x.IsDelete
            }).ToList();

            var pagedResponse = new Paginate<GetTopics>
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

            _cache.Set(cacheKey, pagedResponse,options);
            _courseCacheInvalidator.AddToListCacheKeys(cacheKey);

            return new BaseResponse<Paginate<GetTopics>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Get topics success",
                data = pagedResponse
            };

        }

        public async Task<BaseResponse<string>> UpdateTopics(Guid topicId, EditTopicRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var checkUpdate = await _unitOfWork.GetRepository<Topic>().GetByConditionAsync(x => x.Id == topicId);
                if (checkUpdate == null)
                {
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status404NotFound.ToString(),
                        message = "Topic not found",
                        data = null
                    };
                }

                checkUpdate.Title = request.Title ?? checkUpdate.Title;
                checkUpdate.Description = request.Description ?? checkUpdate.Description;
                checkUpdate.IsDelete = checkUpdate.IsDelete;
                checkUpdate.Duration = request.Duration != 0 ? request.Duration = checkUpdate.Duration : 0;
                _unitOfWork.GetRepository<Topic>().UpdateAsync(checkUpdate);
               await _unitOfWork.CommitAsync();

                var getTopics = await _unitOfWork.GetRepository<Topic>().GetListAsync(predicate: x => x.CourseId == checkUpdate.CourseId);

                var updateCourse = await _unitOfWork.GetRepository<Course>().GetByConditionAsync(x => x.Id == checkUpdate.CourseId);
                updateCourse.Duration = getTopics.Sum(x => x.Duration);

                _unitOfWork.GetRepository<Course>().UpdateAsync(updateCourse);
                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                _topicCacheValidator.InvalidateEntityList();
                _topicCacheValidator.InvalidateEntity(topicId);
                _courseCacheInvalidator.InvalidateEntityList();

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Update Topic Success",
                    data = null
                };
            }
            catch (Exception ex) {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception(ex.ToString());
            }
        }

        public async Task<BaseResponse<string>> DeleteTopics(Guid topicId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var checkDelete = await _unitOfWork.GetRepository<Topic>().GetByConditionAsync(x => x.Id == topicId);
                if (checkDelete == null)
                {
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status404NotFound.ToString(),
                        message = "Topic not found",
                        data = null
                    };
                }

                checkDelete.IsDelete = true;
                _unitOfWork.GetRepository<Topic>().UpdateAsync(checkDelete);
                await _unitOfWork.CommitAsync();

                var getTopics = await _unitOfWork.GetRepository<Topic>().GetListAsync(predicate: x => x.CourseId == checkDelete.CourseId && x.IsDelete == false);

                var updateCourse = await _unitOfWork.GetRepository<Course>().GetByConditionAsync(x => x.Id == checkDelete.CourseId);
                updateCourse.Duration = getTopics.Sum(x => x.Duration);

                _unitOfWork.GetRepository<Course>().UpdateAsync(updateCourse);
                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                _topicCacheValidator.InvalidateEntityList();
                _topicCacheValidator.InvalidateEntity(topicId);
                _courseCacheInvalidator.InvalidateEntityList();

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Delete Topic success",
                    data = null
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception(ex.ToString());
            }
        }
    }
}
