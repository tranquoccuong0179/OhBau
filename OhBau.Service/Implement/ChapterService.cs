using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Chapter;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Chapter;
using OhBau.Model.Payload.Response.Course;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class ChapterService : BaseService<ChapterService>, IChapterService
    {
        public ChapterService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<ChapterService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<string>> CreateChaper(CreateChapterRequest request)
        {
            try
            {
                var createChaper = new Chapter
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Content = request.Content,
                    VideoUrl = request.VideoUrl,
                    ImageUrl = request.ImageUrl,
                    Active = request.Active,
                    CreateAt = DateTime.Now,
                    UpdateAt = null,
                    DeleteAt = null,
                    CourseId = request.CourseId,
                };

                await _unitOfWork.GetRepository<Chapter>().InsertAsync(createChaper);
                await _unitOfWork.CommitAsync();

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

        public async Task<BaseResponse<Paginate<GetChapters>>> GetChaptersByCourse(Guid courseId,int pageNumber, int pageSize, string? title, string? course)
        {
            try
            {
                Expression<Func<Chapter, bool>> predicate = null;
                if (!string.IsNullOrEmpty(title))
                {
                    predicate = x => x.Title.Contains(title) && x.Course.Id == courseId;
                }

                if (!string.IsNullOrEmpty(course))
                {
                    predicate = x => x.Course.Name.Contains(course) && x.Course.Id == courseId;
                }

                var getChapter = await _unitOfWork.GetRepository<Chapter>().GetPagingListAsync(
                                                                                                predicate:predicate,
                                                                                                include: q =>
                                                                                                q.Include(c => c.Course),
                                                                                                page: pageNumber,
                                                                                                size: pageSize);

                var mapItems = getChapter.Items.Select(x => new GetChapters
                {
                    Id = x.Id,
                    Title = x.Title,
                    Content = x.Content
                }).ToList();

                var pagedResponse = new Paginate<GetChapters>
                {
                    Items = mapItems,
                    Page = pageNumber,
                    Size = pageSize,
                    Total = mapItems.Count,
                    TotalPages = getChapter.TotalPages,
                };

                return new BaseResponse<Paginate<GetChapters>>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get Chapter success",
                    data = pagedResponse
                };

            } catch (Exception ex)

            {
                throw new Exception(ex.ToString());   
            }
        }

        public async Task<BaseResponse<GetChapter>> GetChapter(Guid chapterId)
        {
            try
            {
                var getChapter = await _unitOfWork.GetRepository<Chapter>().SingleOrDefaultAsync(predicate: x => x.Id == chapterId,
                                                                                                 include: q => q.Include(c => c.Course));

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
                    Title = getChapter.Title,
                    Content = getChapter.Content,
                    VideoUrl = getChapter.VideoUrl,
                    ImageUrl = getChapter.ImageUrl,
                    Course = getChapter.Course.Name,
                    Active = getChapter.Active,
                    CreateAt = getChapter.CreateAt,
                    UpdateAt = getChapter.UpdateAt,
                    DeleteAt= getChapter.DeleteAt

                };
                return new BaseResponse<GetChapter>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get chapter success",
                    data = mapItem
                };
            }
            catch (Exception ex) { 

                throw new Exception(ex.ToString());
            }

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
                getChapter.CourseId = request.CourseId != null ? request.CourseId : getChapter.CourseId;
                getChapter.Active = request.Active;
                getChapter.CreateAt = getChapter.CreateAt;
                getChapter.UpdateAt = DateTime.Now;
                getChapter.DeleteAt = getChapter.DeleteAt;
                
                _unitOfWork.GetRepository<Chapter>().UpdateAsync(getChapter);
                await _unitOfWork.CommitAsync();

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
