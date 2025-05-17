
using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Exception;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Slot;
using OhBau.Model.Payload.Request.Slot;
using OhBau.Service.Interface;
using OhBau.Model.Paginate;

namespace OhBau.API.Controllers
{
    public class SlotController : BaseController<SlotController>
    {
        private readonly ISlotService _slotService;
        public SlotController(ILogger<SlotController> logger, ISlotService slotService) : base(logger)
        {
            _slotService = slotService;
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

        [HttpGet(ApiEndPointConstant.Slot.GetAllSlot)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<GetSlotResponse>>), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetAllSlot([FromQuery] int? page, [FromQuery] int? size)
        {
            int pageNumber = page ?? 1;
            int pageSize = size ?? 10;
            var response = await _slotService.GetAllSlot(pageNumber, pageSize);
            return StatusCode(int.Parse(response.status), response);
        }

        [HttpGet(ApiEndPointConstant.Slot.GetSlot)]
        [ProducesResponseType(typeof(BaseResponse<GetSlotResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetSlotResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetSlot([FromRoute] Guid id)
        {
            var response = await _slotService.GetSlot(id);
            return StatusCode(int.Parse(response.status), response);
        }
    }
}
