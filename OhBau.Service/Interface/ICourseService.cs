using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Course;
using OhBau.Model.Payload.Request.Topic;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Course;
using OhBau.Model.Payload.Response.Topic;

namespace OhBau.Service.Interface
{
    public interface ICourseService
    {
        Task<BaseResponse<CreateCourseResponse>> CreateCourse(CreateCourseRequest request);
        Task<BaseResponse<Paginate<GetCoursesResponse>>> GetCoursesWithFilterOrSearch(int pageSize, int pageNumber,string? categoryName, string? search);
        Task<BaseResponse<string>> UpdateCourse(Guid courseId, UpdateCourse request);
        Task<BaseResponse<string>> DeleteCourse(Guid courseId);
        Task<BaseResponse<CreateTopicResponse>> CreateTopic(CreateTopicRequest request);
        Task<BaseResponse<Paginate<GetTopics>>> GetTopics(Guid courseId,string? courseName, int pageNumber, int pageSize);
        Task<BaseResponse<string>> UpdateTopics(Guid topicId, EditTopicRequest request);
        Task<BaseResponse<string>> DeleteTopics(Guid topicId);

    }
}
