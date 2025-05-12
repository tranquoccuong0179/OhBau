using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Service.Interface;

namespace OhBau.API.Controllers
{
    [ApiController]
    [Route("api/v1/blog")]
    public class BlogController(IBlogService _blogService, ILogger<BlogController> _logger) : Controller
    {   

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
    }
}
