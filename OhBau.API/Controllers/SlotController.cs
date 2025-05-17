
using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Exception;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Slot;
using OhBau.Model.Payload.Request.Slot;
using OhBau.Service.Interface;
using OhBau.Model.Paginate;
using Microsoft.AspNetCore.Authorization;

namespace OhBau.API.Controllers
{
    public class SlotController : BaseController<SlotController>
    {
        private readonly ISlotService _slotService;
        public SlotController(ILogger<SlotController> logger, ISlotService slotService) : base(logger)
        {
            _slotService = slotService;
        }

        /// <summary>
        /// API cho phép Admin tạo mới một khung giờ (slot) cho phòng khám.
        /// </summary>
        /// <remarks>
        /// - API này cho phép Admin tạo khung giờ với thông tin tên, thời gian bắt đầu, và thời gian kết thúc trong hệ thống phòng khám.
        /// - Các trường bắt buộc trong yêu cầu bao gồm `Name`, `StartTime`, và `EndTime`.
        /// - API yêu cầu xác thực (JWT) và chỉ dành cho người dùng có vai trò Admin.
        /// - Khung giờ được tạo sẽ liên kết với phòng khám được quản lý bởi Admin (dựa trên thông tin từ token hoặc cấu hình hệ thống).
        /// - Ví dụ yêu cầu:
        ///   ```
        ///   POST /api/slot
        ///   Authorization: Bearer &lt;JWT_token&gt;
        ///   Content-Type: application/json
        ///   {
        ///     "name": "Slot1",
        ///     "startTime": "06:00:00",
        ///     "endTime": "07:00:00"
        ///   }
        ///   ```
        /// - Kết quả trả về:
        ///   - `200 OK`: Tạo khung giờ thành công. Trả về `BaseResponse&lt;CreateSlotResponse&gt;` chứa thông tin khung giờ.
        ///   - `400 Bad Request`: Thông tin đầu vào không hợp lệ (ví dụ: `StartTime` lớn hơn hoặc bằng `EndTime`).
        ///   - `401 Unauthorized`: Không cung cấp token hợp lệ.
        ///   - `403 Forbidden`: Người dùng không có quyền Admin.
        /// - Ví dụ phản hồi thành công (200 OK):
        ///   ```json
        ///   {
        ///     "status": "200",
        ///     "message": "Tạo khung giờ thành công",
        ///     "data": {
        ///       "name": "Slot1",
        ///       "startTime": "06:00:00",
        ///       "endTime": "07:00:00"
        ///     }
        ///   }
        ///   ```
        /// - Ví dụ phản hồi lỗi (400 Bad Request):
        ///   ```json
        ///   {
        ///     "status": "400",
        ///     "message": "Dữ liệu đầu vào không hợp lệ",
        ///     "data": null,
        ///     "errors": {
        ///       "EndTime": ["EndTime must be later than StartTime"]
        ///     }
        ///   }
        ///   ```
        /// </remarks>
        /// <param name="request">Thông tin khung giờ cần tạo, bao gồm `Name`, `StartTime`, và `EndTime`.</param>
        /// <returns>
        /// - `200 OK`: Tạo khung giờ thành công.
        /// - `400 Bad Request`: Thông tin đầu vào không hợp lệ.
        /// - `401 Unauthorized`: Không cung cấp token hợp lệ.
        /// - `403 Forbidden`: Người dùng không có quyền Admin.
        /// </returns>
        /// <response code="200">Trả về thông tin khung giờ khi tạo thành công.</response>
        /// <response code="400">Trả về lỗi nếu thông tin đầu vào không hợp lệ.</response>
        /// <response code="401">Trả về lỗi nếu không cung cấp token hợp lệ.</response>
        /// <response code="403">Trả về lỗi nếu người dùng không có quyền Admin.</response>
        [Authorize(Roles = "ADMIN")]
        [HttpPost(ApiEndPointConstant.Slot.CreateSlot)]
        [ProducesResponseType(typeof(BaseResponse<CreateSlotResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CreateSlotResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<CreateSlotResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<CreateSlotResponse>), StatusCodes.Status403Forbidden)]
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
