using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Blog;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Blog;

namespace OhBau.Service.Interface
{
    public interface IBlogService
    {
        Task<BaseResponse<Paginate<GetBlogs>>>GetBlogs(int pageNumber,int pageSize,string? title);  
        Task<BaseResponse<GetBlog>> GetBlog(Guid blogId);
        Task<BaseResponse<CreateNewBlogResponse>> CreateBlog(CreateBlogRequest request);
        Task<BaseResponse<string>> DeleteBlog(Guid blogId);
        Task<BaseResponse<UpdateBlogResponse>> UpdateBlog(Guid id, UpdateBlogRequest request);
        Task<BaseResponse<string>> LikeOrDisLikeBlog(Guid accountId, Guid BlogId);
    }
}
