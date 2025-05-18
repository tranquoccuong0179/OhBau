
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
        [ProducesResponseType(typeof(BaseResponse<List<CreateDoctorSlotResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<List<CreateDoctorSlotResponse>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<List<CreateDoctorSlotResponse>>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> CreateDoctorSlot([FromBody] List<CreateDoctorSlotRequest> request)
        {
            var response = await _doctorSlotService.CreateDoctorSlot(request);
            return StatusCode(int.Parse(response.status), response);
        }

        [HttpPost(ApiEndPointConstant.DoctorSlot.ActiveDoctorSlot)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> ActiveDoctorSlot([FromRoute] Guid id)
        {
            var response = await _doctorSlotService.Active(id);
            return StatusCode(int.Parse(response.status), response);
        }

        [HttpPost(ApiEndPointConstant.DoctorSlot.UnActiveDoctorSlot)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> UnActiveDoctorSlot([FromRoute] Guid id)
        {
            var response = await _doctorSlotService.UnActive(id);
            return StatusCode(int.Parse(response.status), response);
        }
    }
}
