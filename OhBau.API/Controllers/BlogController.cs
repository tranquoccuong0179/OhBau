using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Payload.Request.Blog;
using OhBau.Service.Interface;

namespace OhBau.API.Controllers
{
    [ApiController]
    [Route("api/v1/blog")]
    public class BlogController(IBlogService _blogService, ILogger<BlogController> _logger) : Controller
    {
        [HttpPost(ApiEndPointConstant.Blog.CreateBlog)]
        public async Task<IActionResult> CreateBlog([FromBody] CreateBlogRequest request)
        {
            var response = await _blogService.CreateBlog(request);
            return StatusCode(int.Parse(response.status), response);
        }

        [HttpGet(ApiEndPointConstant.Blog.GetBlogs)]
        public async Task<IActionResult> GetBlogs([FromQuery]int pageNumber, [FromQuery]int pageSize, [FromQuery]string title) 
        {
                var response = await _blogService.GetBlogs(pageNumber, pageSize, title);
                return StatusCode(int.Parse(response.status), response);
        }

        [HttpGet(ApiEndPointConstant.Blog.GetBlog)]
        public async Task<IActionResult> GetBlog([FromQuery]Guid blogId)
        {
            var response = await _blogService.GetBlog(blogId);
            return StatusCode(int.Parse(response.status),response);
        }

        [HttpDelete(ApiEndPointConstant.Blog.DeleteBlog)]
        public async Task<IActionResult> DeleteBlog([FromQuery]Guid blogId)
        {
            var response = await _blogService.DeleteBlog(blogId);
            return StatusCode(int.Parse(response.status), response);
        }

        [HttpPut(ApiEndPointConstant.Blog.UpdateBlog)]
        public async Task<IActionResult> UpdateBlog([FromRoute] Guid id, [FromBody] UpdateBlogRequest request)
        {
            var response = await _blogService.UpdateBlog(id, request);
            return StatusCode(int.Parse(response.status), response);
        }
    }
}
