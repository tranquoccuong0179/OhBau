using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    }
}
