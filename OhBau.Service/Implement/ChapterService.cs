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
using OhBau.Model.Payload.Request.Chapter;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Chapter;
using OhBau.Model.Payload.Response.Course;
using OhBau.Model.Utils;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class ChapterService : BaseService<ChapterService>, IChapterService
    {
        private readonly IMemoryCache _cache;
        private readonly GenericCacheInvalidator<Chapter> _chaperCacheInvalidator;
        private readonly HtmlSanitizerUtil _sanitizer;
        public ChapterService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<ChapterService> logger, IMapper mapper
            , IHttpContextAccessor httpContextAccessor,
            IMemoryCache cache,
            GenericCacheInvalidator<Chapter> chapterCacheInvalidator,
            HtmlSanitizerUtil util) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _cache = cache;
            _chaperCacheInvalidator = chapterCacheInvalidator;
            _sanitizer = util;
        }   

        public async Task<BaseResponse<string>> CreateChaper(CreateChapterRequest request)
        {
            try
            {
                var createChaper = new Chapter
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Content = _sanitizer.Sanitize(request.Content),
                    VideoUrl = request.VideoUrl,
                    ImageUrl = request.ImageUrl,
                    Active = request.Active,
                    CreateAt = DateTime.Now,
                    UpdateAt = null,
                    DeleteAt = null,
                    TopicId = request.TopicId,
                };

                await _unitOfWork.GetRepository<Chapter>().InsertAsync(createChaper);
                await _unitOfWork.CommitAsync();

                _chaperCacheInvalidator.InvalidateEntityList();
                _chaperCacheInvalidator.InvalidateEntity(createChaper.Id);

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Add chapter success",
                    data = null
                };
            }
            catch (Exception ex) { 

                throw new NotImplementedException(ex.ToString());
            }
        }

        public async Task<BaseResponse<Paginate<GetChapters>>> GetChaptersByTopic(Guid topicId,int pageNumber, int pageSize, string? title)
        {
                //var listParameter = new ListParameters<Chapter>(pageNumber, pageSize);
                //listParameter.AddFilter("Title", title);

                //var cacheKey = _chaperCacheInvalidator.GetCacheKeyForList(listParameter);
                //if (_cache.TryGetValue(cacheKey, out Paginate<GetChapters> GetChapters))
                //{
                //    return new BaseResponse<Paginate<GetChapters>>
                //    {
                //        status = StatusCodes.Status200OK.ToString(),
                //        message = "Get chapter success(cache)",
                //        data = GetChapters
                //    };
                //}
                 Expression<Func<Chapter, bool>> predicate = x => x.TopicId == topicId;
                if (!string.IsNullOrEmpty(title))
                {
                predicate = x => x.Topic.Title.Contains(title) && x.Topic.Id == topicId;
                }

                var getChapter = await _unitOfWork.GetRepository<Chapter>().GetPagingListAsync(
                                                                                                predicate:predicate,
                                                                                                include: q =>
                                                                                                q.Include(c => c.Topic),
                                                                                                page: pageNumber,
                                                                                                size: pageSize);

                var mapItems = getChapter.Items.Select(x => new GetChapters
                {
                    Id = x.Id,
                    Image = x.ImageUrl,
                    Title = x.Title,
                    Content = x.Content
                }).ToList();

                var pagedResponse = new Paginate<GetChapters>
                {
                    Items = mapItems,
                    Page = pageNumber,
                    Size = pageSize,
                    Total = mapItems.Count,
                };

                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                };

                //_cache.Set(cacheKey,pagedResponse,options);

                return new BaseResponse<Paginate<GetChapters>>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get Chapter success",
                    data = pagedResponse
                };
        }

        public async Task<BaseResponse<GetChapter>> GetChapter(Guid chapterId)
        {
                var cachedKey = _chaperCacheInvalidator.GetEntityCache<GetChapter>(chapterId);
                if (cachedKey != null)
                {
                    return new BaseResponse<GetChapter>
                    {
                        status = StatusCodes.Status200OK.ToString(),
                        message = "Get chapter success",
                        data = cachedKey
                    };
                }
                var getChapter = await _unitOfWork.GetRepository<Chapter>().SingleOrDefaultAsync(predicate: x => x.Id == chapterId,
                                                                                                 include: q => q.Include(c => c.Topic));

                if (getChapter == null)
                {
                    return new BaseResponse<GetChapter>
                    {
                        status = StatusCodes.Status404NotFound.ToString(),
                        message = "Chapter not found",
                        data = null
                    };
                }

                var mapItem = new GetChapter
                {
                    Id = getChapter.Id,
                    Title = getChapter.Title,
                    Content = getChapter.Content,
                    VideoUrl = getChapter.VideoUrl,
                    ImageUrl = getChapter.ImageUrl,
                    Course = getChapter.Topic.Title,
                    Active = getChapter.Active,
                    CreateAt = getChapter.CreateAt,
                    UpdateAt = getChapter.UpdateAt,
                    DeleteAt= getChapter.DeleteAt

                };

                _chaperCacheInvalidator.SetEntityCache(chapterId, mapItem,TimeSpan.FromMinutes(30));
                return new BaseResponse<GetChapter>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get chapter success",
                    data = mapItem
                };
        }

        public async Task<BaseResponse<string>> UpdateChapter(Guid chapterId, UpdateChapterRequest request)
        {
            try
            {
                var getChapter = await _unitOfWork.GetRepository<Chapter>().GetByConditionAsync(x => x.Id == chapterId);
                if (getChapter == null)
                {
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status200OK.ToString(),
                        message = "Chapter not found",
                        data = null
                    };
                }

                getChapter.Title = request.Title ?? getChapter.Title;
                getChapter.Content = request.Content ?? getChapter.Content;
                getChapter.VideoUrl = request.VideoUrl ?? getChapter.VideoUrl;
                getChapter.ImageUrl = request.ImageUrl ?? getChapter.ImageUrl;
                getChapter.TopicId = request.CourseId != null ? request.CourseId : getChapter.TopicId;
                getChapter.Active = request.Active;
                getChapter.CreateAt = getChapter.CreateAt;
                getChapter.UpdateAt = DateTime.Now;
                getChapter.DeleteAt = getChapter.DeleteAt;
                
                _unitOfWork.GetRepository<Chapter>().UpdateAsync(getChapter);
                await _unitOfWork.CommitAsync();

                _chaperCacheInvalidator.InvalidateEntity(chapterId);
                _chaperCacheInvalidator.InvalidateEntityList();

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Update chapter success",
                    data = null
                };
            }
            catch (Exception ex) { 
                
                throw new Exception(ex.ToString());
            }
        }

        public async Task<BaseResponse<string>>DeleteChapter(Guid chapterId)
        {
            try
            {
                var checkDelete = await _unitOfWork.GetRepository<Chapter>().GetByConditionAsync(x => x.Id == chapterId);
                if (checkDelete == null)
                {
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status404NotFound.ToString(),
                        message = "Chapter not found",
                        data = null
                    };
                }

                checkDelete.Active = false;
                checkDelete.CreateAt = DateTime.Now;
                _unitOfWork.GetRepository<Chapter>().UpdateAsync(checkDelete);
                await _unitOfWork.CommitAsync();
                    
                _chaperCacheInvalidator.InvalidateEntity(chapterId);
                _chaperCacheInvalidator.InvalidateEntityList();

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Delete chapter success",
                    data = null
                };

            }
            catch (Exception ex) { 
            
               throw new Exception(ex.ToString(), ex);
            }
        }
    }
}
