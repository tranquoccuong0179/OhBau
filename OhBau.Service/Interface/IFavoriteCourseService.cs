using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.FavoriteCourse;

namespace OhBau.Service.Interface
{
    public interface IFavoriteCourseService
    {
        Task<BaseResponse<string>> AddDeleteFavoriteCourse(Guid accountId, Guid courseId);
        Task<BaseResponse<Paginate<FavoriteCoursesResponse>>> GetFavoriteCourse(int pageNumber, int pageSize,Guid accountId, string? courseName, string? category);
    }
}
