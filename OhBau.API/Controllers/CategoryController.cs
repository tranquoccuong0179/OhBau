using Microsoft.AspNetCore.Mvc;
using OhBau.Model.Payload.Request.Category;
using OhBau.Service.Interface;

namespace OhBau.API.Controllers
{
    [ApiController]
    [Route("api/v1/category")]
    public class CategoryController(ILogger<CategoryController> _logger, ICategoryService _categoryService) : Controller
    {
        [HttpGet("get-categories")]
        public async Task<IActionResult> GetCategories([FromQuery]int pageNumber, [FromQuery] int pageSize)
        {
            var response = await _categoryService.GetCategories(pageNumber, pageSize);
            return StatusCode(int.Parse(response.status), response);
        }

        [HttpPut("edit-category{categoryId}")]
        public async Task<IActionResult> EditCategory(Guid categoryId, [FromBody]EditCategoryRequest request)
        {
            try
            {
                var response = await _categoryService.EditCategory(categoryId, request);
                return StatusCode(int.Parse(response.status),response);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Edit category API] "  + ex.Message,ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpDelete("delete-category{categoryId}")]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            try
            {
                var response = await _categoryService.DeleteCategory(categoryId);
                return StatusCode(int.Parse(response.status), response);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Delete category API] " + ex.Message, ex.StackTrace);
                return StatusCode(500, ex?.ToString());
            }
        }
    }
}
