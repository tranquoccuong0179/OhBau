using Microsoft.AspNetCore.Mvc;
using OhBau.Model.Payload.Request.Fetus;
using OhBau.Service.Interface;

namespace OhBau.API.Controllers
{
    [ApiController]
    [Route("api/v1/fetus")]
    public class FetusController(IDoctorService _doctorService, ILogger<FetusController> _logger) : Controller
    {
        [HttpPut("edit-fetus-information/{fetusId}")]
        public async Task<IActionResult> EditFetusInformation(Guid fetusId, [FromBody] EditFetusInformationRequest request)
        {
            try
            {
                if (!ModelState.IsValid) {

                    return BadRequest(ModelState);
                }

                var response = await _doctorService.EditFetusInformation(fetusId,request);
                return StatusCode(int.Parse(response.status), response);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Edit Fetus Information API] " + ex.Message, ex.StackTrace,ex.ToString());
                return StatusCode(500, ex.ToString());
            }
        }
    }
}
