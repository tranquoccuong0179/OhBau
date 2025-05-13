using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Payload.Request.Blog;
using OhBau.Model.Payload.Response.Blog;
using OhBau.Model.Payload.Response;
using OhBau.Service.Interface;

namespace OhBau.API.Controllers
{
    [ApiController]
    [Route("api/v1/blog")]
    public class BlogController(IBlogService _blogService, ILogger<BlogController> _logger) : Controller
    {

        /// <summary>
        /// API tạo mới bài viết blog.
        /// </summary>
        /// <remarks>
        /// - API này cho phép tạo mới bài viết blog dựa trên `CreateBlogRequest`.
        /// - Tất cả các trường trong `CreateBlogRequest` (`Title`, `Content`) đều bắt buộc.
        /// - Với `Content` thì bạn nên sử dụng thêm api:
        ///   ```
        ///   POST /api/v1/upload/upload
        ///   ```
        /// - Để thêm hình, lưu link và tag `img` để thêm vào content 
        /// - Yêu cầu xác thực (người dùng phải đăng nhập và có quyền tạo blog).
        /// - Ví dụ yêu cầu:
        ///   ```
        ///   POST /api/v1/blog/create
        ///   ```
        /// - Ví dụ nội dung yêu cầu:
        ///   ```json
        ///   {
        ///     "title": "Tiêu đề blog mới",
        ///     "content": "Nội dung blog mới"
        ///   }
        ///   ```
        /// - Kết quả trả về:
        ///   - `200 OK`: Tạo bài viết thành công. Trả về `BaseResponse&lt;CreateNewBlogResponse&gt;` chứa thông tin blog vừa tạo.
        ///   - `400 Bad Request`: Yêu cầu không hợp lệ (ví dụ: thiếu `Title` hoặc `Content`, hoặc dữ liệu đầu vào không hợp lệ).
        ///   - `401 Unauthorized`: Người dùng chưa đăng nhập hoặc không có quyền tạo blog.
        /// - Ví dụ phản hồi thành công (200 OK):
        ///   ```json
        ///   {
        ///     "status": "200",
        ///     "data": {
        ///       "id": "123e4567-e89b-12d3-a456-426614174000",
        ///       "title": "Tiêu đề blog mới",
        ///       "content": "Nội dung blog mới",
        ///       "status": "Draft"
        ///     },
        ///     "message": "Đăng kí blog thành công"
        ///   }
        ///   ```
        /// </remarks>
        /// <param name="request">Thông tin yêu cầu tạo blog. Phải bao gồm `Title` và `Content`.</param>
        /// <returns>
        /// - `200 OK`: Tạo bài viết thành công.
        /// - `400 Bad Request`: Yêu cầu không hợp lệ.
        /// - `401 Unauthorized`: Người dùng chưa đăng nhập hoặc không có quyền.
        /// </returns>
        /// <response code="200">Trả về kết quả khi bài viết blog được tạo thành công.</response>
        /// <response code="400">Trả về lỗi nếu yêu cầu không hợp lệ.</response>
        /// <response code="401">Trả về lỗi nếu người dùng chưa đăng nhập hoặc không có quyền.</response>
        [HttpPost(ApiEndPointConstant.Blog.CreateBlog)]
        [ProducesResponseType(typeof(BaseResponse<CreateNewBlogResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CreateNewBlogResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<CreateNewBlogResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
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

        /// <summary>
        /// API cập nhật thông tin bài viết blog.
        /// </summary>
        /// <remarks>
        /// - API này cho phép cập nhật thông tin bài viết blog (tiêu đề và nội dung) dựa trên `UpdateBlogRequest`.
        /// - Các trường trong `UpdateBlogRequest` (`Title`, `Content`) là tùy chọn. Nếu trường là `null`, giá trị cũ sẽ được giữ nguyên.
        /// - Yêu cầu xác thực (người dùng phải đăng nhập và có quyền chỉnh sửa blog).
        /// - Ví dụ yêu cầu:
        ///   ```
        ///   PUT /api/v1/blog/update-blog/{id}
        ///   ```
        /// - Ví dụ nội dung yêu cầu:
        ///   ```json
        ///   {
        ///     "title": "Tiêu đề blog đã cập nhật",
        ///     "content": "Nội dung blog đã cập nhật"
        ///   }
        ///   ```
        /// - Kết quả trả về:
        ///   - `200 OK`: Cập nhật thành công. Trả về `BaseResponse&lt;UpdateBlogResponse&gt;` chứa thông tin blog đã cập nhật.
        ///   - `400 Bad Request`: Yêu cầu không hợp lệ (ví dụ: `request` là `null` hoặc dữ liệu đầu vào không hợp lệ).
        ///   - `401 Unauthorized`: Người dùng chưa đăng nhập hoặc không có quyền chỉnh sửa.
        ///   - `403 Forbidden`: Người dùng không có quyền chỉnh sửa blog.
        ///   - `404 NotFound`: Không tìm thấy bài viết blog với `id` cung cấp.
        /// - Ví dụ phản hồi thành công (200 OK):
        ///   ```json
        ///   {
        ///     "status": "200",
        ///     "data": {
        ///       "id": "123e4567-e89b-12d3-a456-426614174000",
        ///       "title": "Tiêu đề blog đã cập nhật",
        ///       "content": "Nội dung blog đã cập nhật",
        ///       "status": "Published"
        ///     },
        ///     "message": "Cập nhật blog thành công"
        ///   }
        ///   ```
        /// </remarks>
        /// <param name="id">ID của bài viết blog cần cập nhật.</param>
        /// <param name="request">Thông tin yêu cầu cập nhật blog. Có thể bao gồm `Title` và `Content` (tùy chọn).</param>
        /// <returns>
        /// - `200 OK`: Cập nhật thành công.
        /// - `400 Bad Request`: Yêu cầu không hợp lệ.
        /// - `401 Unauthorized`: Người dùng chưa đăng nhập hoặc không có quyền.
        /// - `403 Forbidden`: Người dùng không có quyền chỉnh sửa blog.
        /// - `404 NotFound`: Bài viết blog không tồn tại.
        /// </returns>
        /// <response code="200">Trả về kết quả khi bài viết blog được cập nhật thành công.</response>
        /// <response code="400">Trả về lỗi nếu yêu cầu không hợp lệ.</response>
        /// <response code="401">Trả về lỗi nếu người dùng chưa đăng nhập.</response>
        /// <response code="403">Trả về lỗi nếu người dùng không có quyền chỉnh sửa blog.</response>
        /// <response code="404">Trả về lỗi nếu bài viết blog không tồn tại.</response>
        [HttpPut(ApiEndPointConstant.Blog.UpdateBlog)]
        [ProducesResponseType(typeof(BaseResponse<UpdateBlogResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<UpdateBlogResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<UpdateBlogResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<UpdateBlogResponse>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BaseResponse<UpdateBlogResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateBlog([FromRoute] Guid id, [FromBody] UpdateBlogRequest request)
        {
            var response = await _blogService.UpdateBlog(id, request);
            return StatusCode(int.Parse(response.status), response);
        }
    }
}
