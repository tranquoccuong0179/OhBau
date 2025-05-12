using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Blog;

namespace OhBau.Service.Interface
{
    public interface IBlogService
    {
        Task<BaseResponse<Paginate<GetBlogs>>>GetBlogs(int pageNumber,int pageSize,string? title);  
        Task<BaseResponse<GetBlog>> GetBlog(Guid blogId);   

        Task<BaseResponse<string>> DeleteBlog(Guid blogId);
    }
}
