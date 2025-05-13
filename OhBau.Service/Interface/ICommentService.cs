using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Comment;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Comment;

namespace OhBau.Service.Interface
{
    public interface ICommentService
    {
        Task<BaseResponse<string>> Comment(Guid accountId,CommentRequest request);
        Task<BaseResponse<string>> ReplyComment(Guid accountId,ReplyComment request);
        Task<BaseResponse<Paginate<GetComments>>> GetComments(Guid blogId,int pageNumber, int pageSize);
        Task<BaseResponse<string>> EditComment(Guid accountId, EditComment request);

        Task<BaseResponse<string>> DeleteComment(Guid accountId,Guid commentId);
    }
}
