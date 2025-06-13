using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OhBau.Model.Payload.Request.Order;
using OhBau.Model.Utils;
using OhBau.Service.Interface;

namespace OhBau.API.Controllers
{
    [ApiController]
    [Route("api/v1/my-course")]
    public class MyCourseController(ILogger<MyCourseController> _logger, IMyCourseService _myCourseService) : Controller
    {
        [HttpGet("my-course")]
        [Authorize(Roles = "FATHER,MOTHER")]
        public async Task<IActionResult> MyCourses([FromQuery]int pageNumber,[FromQuery]int pageSize, [FromQuery] string? courseName)
        {
            var accountId = UserUtil.GetAccountId(HttpContext);
            var response = await _myCourseService.MyCourses(accountId!.Value,pageNumber,pageSize,courseName);
            return StatusCode(int.Parse(response.status), response);
        }

        [HttpPost("receive-course")]
        [Authorize(Roles = "FATHER,MORTHER")]
        public async Task<IActionResult> ReceiveCourse([FromBody] AddCourseToOrderRequest reqeust)
        {
            try
            {
                var accountId = UserUtil.GetAccountId(HttpContext);
                var response = await _myCourseService.ReceiveCourse(accountId!.Value, reqeust.CourseId);
                return StatusCode(int.Parse(response.status), response);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Receive Course API] " + ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }
    }
}
