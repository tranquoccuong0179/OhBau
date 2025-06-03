using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.MyCourse;

namespace OhBau.Service.Interface
{
    public interface IMyCourseService
    {
        Task<BaseResponse<Paginate<MyCoursesResponse>>> MyCourses(Guid accountId,int pageNumber, int pageSize, string? courseName);
        Task<BaseResponse<string>> ReceiveCourse(Guid accountId, Guid courseId);
    }
}
