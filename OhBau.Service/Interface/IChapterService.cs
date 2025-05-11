using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Chapter;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Chapter;
using OhBau.Model.Payload.Response.Course;

namespace OhBau.Service.Interface
{
    public interface IChapterService
    {
        Task<BaseResponse<string>> CreateChaper(CreateChapterRequest request);
        Task<BaseResponse<Paginate<GetChapters>>> GetChaptersByCourse(Guid courseId, int pageNumber, int pageSize,string? title, string? course);
        Task<BaseResponse<GetChapter>> GetChapter(Guid chapterId);
        Task<BaseResponse<string>> UpdateChapter(Guid chapterId, UpdateChapterRequest request);
        Task<BaseResponse<string>> DeleteChapter(Guid chapterId);
    }
}
