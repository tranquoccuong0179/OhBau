using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Course;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Course;

namespace OhBau.Service.Interface
{
    public interface ICourseService
    {
        Task<BaseResponse<CreateCourseResponse>> CreateCourse(CreateCourseRequest request);
        Task<BaseResponse<Paginate<GetCoursesResponse>>> GetCoursesWithFilterOrSearch(int pageSize, int pageNumber,string? categoryName, string? search);
    }
}
