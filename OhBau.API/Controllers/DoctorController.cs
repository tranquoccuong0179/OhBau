using Microsoft.AspNetCore.Mvc;
using OhBau.Model.Payload.Request.Doctor;
using OhBau.Model.Payload.Request.Major;
using OhBau.Service.Interface;

namespace OhBau.API.Controllers
{
    [ApiController]
    [Route("api/v1/doctor")]
    public class DoctorController(IDoctorService _doctorService, ILogger<DoctorController> _logger) : Controller
    {


        [HttpPost("create-major")]
        public async Task<IActionResult> CreateMajor([FromBody] CreateMajorRequest request)
        {
            try
            {
                var response = await _doctorService.CreateMajonr(request);
                return StatusCode(int.Parse(response.status), response);
            }

            catch (Exception ex) {
                _logger.LogError($"Exception : " +  ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }


        [HttpPost("create-doctor")]
        public async Task<IActionResult> CreateDoctor([FromBody]CreateDoctorRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _doctorService.CreateDoctor(request);
                return StatusCode(int.Parse(response.status), response);
            }
            catch (Exception ex) { 
            
                _logger.LogError(ex.Message, ex.StackTrace);
                return StatusCode(500,ex.ToString());
            }
        }

        [HttpGet("get-doctors")]
        public async Task<IActionResult> GetDoctor([FromQuery]int pageSize, [FromQuery]int pageNumber)
        {
            try
            {
                var response = await _doctorService.GetDoctors(pageSize, pageNumber);
                return StatusCode(int.Parse(response.status), response);
            }
            catch (Exception ex) { 
                
                _logger.LogError(ex, ex.StackTrace);
                return StatusCode(500, ex.ToString() );
            }
        }

        [HttpGet("get-doctor-infor")]
        public async Task<IActionResult> GetDoctorInfor([FromQuery] Guid doctorID)
        {
            try
            {
                var response = await _doctorService.GetDoctorInfo(doctorID);
                return StatusCode(int.Parse(response.status),response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Get Docotr Infor] " + ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }

    }
}
