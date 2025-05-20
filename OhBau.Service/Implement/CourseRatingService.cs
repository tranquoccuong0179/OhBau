using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Payload.Request.Like;
using OhBau.Model.Payload.Response;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class CourseRatingService : BaseService<CourseRatingService>, ICourseRating
    {
        public CourseRatingService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<CourseRatingService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<string>> Rating(Guid accountId, RatingRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var checkMyCourse = await _unitOfWork.GetRepository<MyCourse>().SingleOrDefaultAsync(
                    predicate: x => x.AccountId == accountId && x.CourseId == request.CourseId
                    );

                if(checkMyCourse == null)
                {
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status403Forbidden.ToString(),
                        message = "You must purchase the course to be able to rate it.",
                        data = null
                    };
                }

                var rating = new CourseRating
                {
                    AccountId = accountId,
                    CourseId = request.CourseId,
                    Rating = request.Rating,
                    Description = request.Description
                };

                await _unitOfWork.GetRepository<CourseRating>().InsertAsync(rating);
                await _unitOfWork.CommitAsync();

                var getCourse = await _unitOfWork.GetRepository<Course>().SingleOrDefaultAsync(
                    predicate: x => x.Id == request.CourseId
                    );

                if (getCourse != null)
                {
                    var getRatingByCourse = await _unitOfWork.GetRepository<CourseRating>().GetListAsync(
                        predicate: x => x.CourseId == request.CourseId
                        );

                    getCourse.Rating = getRatingByCourse.Average(x => x.Rating);
                    _unitOfWork.GetRepository<Course>().UpdateAsync(getCourse);
                    await _unitOfWork.CommitAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status200OK.ToString(),
                        message = "Rating Success",
                        data = null
                    };
                }

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status404NotFound.ToString(),
                    message = "Course not found",
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
