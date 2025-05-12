using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Payload.Request.Comment;
using OhBau.Model.Utils;
using OhBau.Service.Interface;

namespace OhBau.API.Controllers
{
    [ApiController]
    [Route("api/v1/comment")]
    public class CommentController(ICommentService _commentService, ILogger<CommentController> _logger) : Controller
    {
        
        [HttpPost(ApiEndPointConstant.Comment.CreateComment)]
        [Authorize]
        public async Task<IActionResult> CreateComment([FromBody] CommentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var accountId = UserUtil.GetAccountId(HttpContext);
                var response = await _commentService.Comment(accountId!.Value, request);
                return StatusCode(int.Parse(response.status), response);

            }
            catch (Exception ex) { 
                _logger.LogError("[Comment API] " + ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPost(ApiEndPointConstant.Comment.Reply)]
        [Authorize]
        public async Task<IActionResult> ReplyComment([FromBody] ReplyComment request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var accountId = UserUtil.GetAccountId(HttpContext);
                var response = await _commentService.ReplyComment(accountId!.Value, request);
                return StatusCode(int.Parse(response.status),response);
            }
            catch (Exception ex) { 

                _logger.LogError("[Reply Comment API] " + ex.Message, ex.StackTrace);
                return StatusCode(500,ex.ToString());
            }
        }
        [HttpGet(ApiEndPointConstant.Comment.GetComments)]
        public async Task<IActionResult> GetComments([FromQuery]Guid BlogId, [FromQuery]int pageNumber, [FromQuery]int pageSize)
        {
            var response = await _commentService.GetComments(BlogId, pageNumber,pageSize);
            return StatusCode(int.Parse(response.status), response);

        }

        [HttpPut("edit-comment/{commentId}")]
        [Authorize]
        public async Task<IActionResult> EditComment(Guid commentId, [FromBody]EditComment request)
        {
            try
            {
                var accountId = UserUtil.GetAccountId(HttpContext);
                request.CommentId = commentId;
                var response = await _commentService.EditComment(accountId!.Value, request);
                return StatusCode(int.Parse(response.status), response);
            }
            catch (Exception ex) { 
                _logger.LogError("[Edit comment API] " + ex.Message,ex.StackTrace);
                return StatusCode(500,ex.ToString());
            }
        }

        [HttpDelete("delete-comment/{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            try
            {
                var accountId = UserUtil.GetAccountId(HttpContext);
                var response = await _commentService.DeleteComment(accountId!.Value,commentId);
                return StatusCode(int.Parse(response.status), response);
            }
            catch (Exception ex)
            {

                _logger.LogError("[Delete comment API] " + ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }
    }
}
