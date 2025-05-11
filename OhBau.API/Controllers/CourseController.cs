using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OhBau.Model.Payload.Request.Course;
using OhBau.Service.Interface;

namespace OhBau.API.Controllers
{
    [ApiController]
    [Route("api/v1/course")]
    public class CourseController(ICourseService _courseService, ILogger<CourseController> _logger) : Controller
    {
        [HttpPost("create-course")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _courseService.CreateCourse(request);
                return StatusCode(int.Parse(response.status), response);
            }
            catch (Exception ex)
            {

                _logger.LogError("[Create Course API] " + ex.Message, ex.StackTrace, ex.ToString());
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("get-courses")]
        public async Task<IActionResult> GetCourse([FromQuery] int pageSize, [FromQuery] int pageNumber, [FromQuery] string? CategoryName, [FromQuery] string? Name)
        {
            try
            {
                var reponse = await _courseService.GetCoursesWithFilterOrSearch(pageSize, pageNumber, CategoryName, Name);
                return StatusCode(int.Parse(reponse.status), reponse);
            }
            catch (Exception ex)
            {

                _logger.LogError("[Get Courese API ]" + ex.ToString(), ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPut("update-course{courseId}")]
        public async Task<IActionResult> UpdateCourse(Guid courseId, [FromBody]UpdateCourse request)
        {
            try
            {
                var response = await _courseService.UpdateCourse(courseId, request);
                return StatusCode(int.Parse(response.status),response);
            }
            catch (Exception ex) { 

                _logger.LogError("[Update Course API] " + ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpDelete("delte-course{courseId}")]
        public async Task<IActionResult> DeleteCourse([FromQuery] Guid courseId)
        {
            try
            {
                var response = await _courseService.DeleteCourse(courseId);
                return StatusCode(int.Parse(response.status), response);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Delete Course API] " + ex.Message, ex, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }
    }
}
