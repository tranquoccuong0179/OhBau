using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OhBau.Model.Payload.Request.Like;
using OhBau.Model.Utils;
using OhBau.Service.Interface;

namespace OhBau.API.Controllers
{
    [ApiController]
    [Route("api/v1/course-rating")]
    public class LikesController(ILogger<LikesController> _logger, ICourseRating _ratingCourseService) : Controller
    {
        [HttpPost("rating")]
        [Authorize(Roles = "MOTHER,FATHER")]
        public async Task<IActionResult> Rating([FromBody] RatingRequest request)
        {
            try
            {
                var accountId = UserUtil.GetAccountId(HttpContext);
                var response = await _ratingCourseService.Rating(accountId!.Value, request);
                return StatusCode(int.Parse(response.status), response);
            }
            catch (Exception ex) {

                _logger.LogError("[API Like ]" + ex.Message, ex.StackTrace);
               return StatusCode(500, ex.ToString());
            }
        }
      
    }
}
