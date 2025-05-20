using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OhBau.Model.Payload.Request.FavoriteCourse;
using OhBau.Model.Utils;
using OhBau.Service.Interface;

namespace OhBau.API.Controllers
{
    [ApiController]
    [Route("api/v1/favorite-course")]
    public class FavoriteCoursesController(ILogger<FavoriteCoursesController> _logger, IFavoriteCourseService _favoriteCourseService) : Controller
    {
        [HttpPost("add-favorite-course")]
        [Authorize(Roles = "FATHER,MOTHER")]
        public async Task<IActionResult> AddFavoriteCourse([FromBody]AddFavoriteCourseRequest request)
        {
            try
            {
                var accountId = UserUtil.GetAccountId(HttpContext);
                
                var response = await _favoriteCourseService.AddDeleteFavoriteCourse(accountId!.Value, request.CourseId);
                return StatusCode(int.Parse(response.status), response);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Add Favorite course API]" +  ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("get-favorite-courses")]
        [Authorize(Roles = "FATHER,MOHTER")]
        public async Task<IActionResult> GetFavoriteCourse([FromQuery]int pageNumber, [FromQuery]int pageSize, [FromQuery]string? courseName,
            [FromQuery] string? categoryName)
        {
            var accountId = UserUtil.GetAccountId(HttpContext);
            var response = await _favoriteCourseService.GetFavoriteCourse(pageNumber,pageSize, accountId!.Value, courseName,categoryName);
            return StatusCode(int.Parse(response.status),response);
        }
    }
}
