using Microsoft.AspNetCore.Mvc;
using OhBau.Model.Payload.Request.Upload;
using OhBau.Service.CloudinaryService;

namespace OhBau.API.Controllers
{
    [ApiController]
    [Route("api/v1/upload")]
    public class UploadController(ICloudinaryService _cloudinaryService, ILogger<UploadController> _logger) : Controller
    {

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm]UploadRequest request)
        {
            try
            {
                var response = await _cloudinaryService.Upload(request.file);
                return StatusCode(int.Parse(response.status),response);
            }
            catch (Exception ex) { 
            
                _logger.LogError("[Upload API] " + ex.Message,ex.StackTrace);
                return StatusCode(500,ex.ToString());
            }
        }

        [HttpPost("upload-video")] 
        public async Task<IActionResult> UploadVideo([FromForm]UploadRequest request)
        {
            try
            {
                var response = await _cloudinaryService.UploadVideo(request.file);
                return StatusCode(int.Parse(response.status), response);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Upload Video API] " + ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }
    }
}
