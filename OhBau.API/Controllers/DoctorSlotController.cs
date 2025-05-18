
using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Payload.Response;
using OhBau.Service.Interface;
using OhBau.Model.Payload.Response.DoctorSlot;
using OhBau.Model.Payload.Request.DoctorSlot;

namespace OhBau.API.Controllers
{
    public class DoctorSlotController : BaseController<DoctorSlotController>
    {
        private readonly IDoctorSlotService _doctorSlotService;
        public DoctorSlotController(ILogger<DoctorSlotController> logger, IDoctorSlotService doctorSlotService) : base(logger)
        {
            _doctorSlotService = doctorSlotService;
        }

        [HttpPost(ApiEndPointConstant.DoctorSlot.CreateDoctorSlot)]
        [ProducesResponseType(typeof(BaseResponse<CreateDoctorSlotReponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CreateDoctorSlotReponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<CreateDoctorSlotReponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> CreateDoctorSlot([FromBody] CreateDoctorSlotRequest request)
        {
            var response = await _doctorSlotService.CreateDoctorSlot(request);
            return StatusCode(int.Parse(response.status), response);
        }
    }
}
