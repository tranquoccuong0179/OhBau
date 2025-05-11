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
using OhBau.Model.Payload.Request.Course;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Course;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class CourseService : BaseService<CourseService>, ICourseService
    {
        public CourseService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<CourseService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
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
                 _unitOfWork.GetRepository<Course>().DeleteAsync(checkDelele);
                await _unitOfWork.CommitAsync();

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

            return new BaseResponse<Paginate<GetCoursesResponse>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Get course success",
                data = pagedReponse
            };

            throw new NotImplementedException();
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
                getCourse.Duration = request.Duration != 0 ? request.Duration : getCourse.Duration;
                getCourse.Price = request.Price != 0 ? request.Price : getCourse.Duration;
                getCourse.CategoryId = request.CategoryId != null ? request.CategoryId : getCourse.CategoryId;
                getCourse.Active = request.Active != null ? request.Active : getCourse.Active;
                getCourse.CreateAt = getCourse.CreateAt;
                getCourse.UpdateAt = getCourse.UpdateAt;
                getCourse.DeleteAt = getCourse.DeleteAt;

                _unitOfWork.GetRepository<Course>().UpdateAsync(getCourse);
                await _unitOfWork.CommitAsync();

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
    }
}
