using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Payload.Request.Chapter;
using OhBau.Service.Interface;

namespace OhBau.API.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class ChapterController(IChapterService _chapterService, ILogger<ChapterController> _logger) : Controller
    {
        [HttpPost(ApiEndPointConstant.Chapter.AddChapter)]
        public async Task<IActionResult> AddChapter([FromBody] CreateChapterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

               var response = await _chapterService.CreateChaper(request);
               return StatusCode(int.Parse(response.status),response);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Add Chapter API] " + ex.Message,ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }


        [HttpGet(ApiEndPointConstant.Chapter.GetChapters)]
        public async Task<IActionResult> GetChapters([FromQuery]Guid courseId, [FromQuery]int pageNumber, [FromQuery]int pageSize, [FromQuery]string?title, string? courseName)
        {
            try
            {
                var response = await _chapterService.GetChaptersByCourse(courseId, pageNumber, pageSize, title, courseName);
                return StatusCode(int.Parse(response.status), response);
            }
            catch (Exception ex) {

                _logger.LogError("[Get Chapters API]" + ex.Message, ex.StackTrace);
                return StatusCode(500,ex.ToString());
            }
        }

        [HttpGet(ApiEndPointConstant.Chapter.GetChapter)]
        public async Task<IActionResult> GetChapter([FromQuery] Guid chapterId)
        {
            try
            {
                var response = await _chapterService.GetChapter(chapterId);
                return StatusCode(int.Parse(response.status),response);
            }
            catch (Exception ex)
            {

                _logger.LogError("[Get Chapter API] " + ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPut("update-chapter/{chapterId}")]
        public async Task<IActionResult> UpdateChapter(Guid chapterId, [FromBody] UpdateChapterRequest request)
        {
            try
            {
               var response = await _chapterService.UpdateChapter(chapterId, request);
               return StatusCode(int.Parse(response.status),response);
            }
            catch (Exception ex) { 
                
                _logger.LogError("[Update Chapter API]" + ex.Message,ex.StackTrace);
                return StatusCode(500,ex.ToString());
            }
        }

        [HttpDelete("delete-chapter")]
        public async Task<IActionResult> DeleteChapter([FromQuery]Guid chapterId)
        {
            try
            {
                var response = await _chapterService.DeleteChapter(chapterId);
                return StatusCode(int.Parse(response.status), response);
            }
            catch (Exception ex) {

                _logger.LogError("[Delte Chapter API] " + ex.Message,ex.StackTrace);
                return StatusCode(500,ex.ToString());
            }
        }

    }
}
