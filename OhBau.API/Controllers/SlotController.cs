
using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Exception;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Slot;
using OhBau.Model.Payload.Request.Slot;
using OhBau.Service.Interface;

namespace OhBau.API.Controllers
{
    public class SlotController : BaseController<SlotController>
    {
        private readonly ISlotService _slotService;
        public SlotController(ILogger<SlotController> logger, ISlotService slotService) : base(logger)
        {
            _slotService = _slotService;
        }

        [HttpPost(ApiEndPointConstant.Slot.CreateSlot)]
        [ProducesResponseType(typeof(BaseResponse<CreateSlotResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CreateSlotResponse>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> CreateSlot([FromBody] CreateSlotRequest request)
        {
            var response = await _slotService.CreateSlot(request);
            return StatusCode(int.Parse(response.status), response);
        }
    }
}
