using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OhBau.Model.Payload.Request.Course;
using OhBau.Model.Payload.Request.Topic;
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
             var reponse = await _courseService.GetCoursesWithFilterOrSearch(pageSize, pageNumber, CategoryName, Name);
             return StatusCode(int.Parse(reponse.status), reponse);
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
        public async Task<IActionResult> DeleteCourse(Guid courseId)
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

        [HttpPost("create-topic")]
        public async Task<IActionResult> CreateTopic([FromBody]CreateTopicRequest request)
        {
            try
            {
                var response = await _courseService.CreateTopic(request);
                return StatusCode(int.Parse(response.status), response);
            }
            catch (Exception ex) { 
             
                _logger.LogError("[Create Topic]" + ex.Message,ex.StackTrace);
                return StatusCode(500,ex.ToString());
            }
        }

        [HttpGet("get-topics")]
        public async Task<IActionResult> GetTopics([FromQuery]Guid courseId, [FromQuery]string? courseName, [FromQuery]int pageNumber, [FromQuery]int pageSize)
        {
            var response = await _courseService.GetTopics(courseId, courseName, pageNumber, pageSize);
            return StatusCode(int.Parse(response.status), response);
        }

        [HttpPut("edit-topic{topicId}")]
        public async Task<IActionResult> EditTopic(Guid topicId, [FromBody] EditTopicRequest request)
        {
            try
            {
                var response = await _courseService.UpdateTopics(topicId, request);
                return StatusCode(int.Parse(response.status),response);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Edit Topic API] " + ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpDelete("delete-topic")]
        public async Task<IActionResult> DeleteTopic([FromQuery]Guid topicId)
        {
            try
            {
                var response = await _courseService.DeleteTopics(topicId);
                return StatusCode(int.Parse(response.status), response);
            }
            catch (Exception ex) { 
                _logger.LogError("[Delete Topic API] " + ex.Message, ex.StackTrace);
                return StatusCode(500,ex.ToString());
            }
        }
    }
}
