
using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Payload.Request.MotherHealth;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.MotherHealth;
using OhBau.Service.Interface;

namespace OhBau.API.Controllers
{
    public class MotherHealthController : BaseController<MotherHealthController>
    {
        private readonly IMotherHealthService _motherHealthService;
        public MotherHealthController(ILogger<MotherHealthController> logger, IMotherHealthService motherHealthService) : base(logger)
        {
            _motherHealthService = motherHealthService;
        }

        /// <summary>
        /// API cập nhật thông tin sức khỏe của mẹ.
        /// </summary>
        /// <remarks>
        /// - API này cho phép cập nhật thông tin sức khỏe của mẹ (cân nặng và huyết áp) dựa trên `UpdateMotherHealthRequest`.
        /// - Các trường trong `UpdateMotherHealthRequest` (`Weight`, `BloodPressure`) là tùy chọn. Nếu trường là `null`, giá trị cũ sẽ được giữ nguyên.
        /// - Yêu cầu xác thực (người dùng phải đăng nhập để sử dụng API này).
        /// - Ví dụ yêu cầu:
        ///   ```
        ///   PUT /api/v1/mother-health/{id}
        ///   ```
        /// - Ví dụ nội dung yêu cầu:
        ///   ```json
        ///   {
        ///     "weight": 65.5,
        ///     "bloodPressure": 120.0
        ///   }
        ///   ```
        /// - Kết quả trả về:
        ///   - `200 OK`: Cập nhật thành công. Trả về `BaseResponse&lt;UpdateMotherHealthResponse&gt;` chứa thông tin sức khỏe đã cập nhật.
        ///   - `400 Bad Request`: Yêu cầu không hợp lệ (ví dụ: `request` là giá trị đầu vào ngoài phạm vi hợp lệ).
        ///   - `401 Unauthorized`: Người dùng chưa đăng nhập hoặc không có quyền truy cập.
        ///   - `404 NotFound`: Không tìm thấy bản ghi sức khỏe của mẹ với `id` cung cấp.
        /// - Ví dụ phản hồi thành công (200 OK):
        ///   ```json
        ///   {
        ///     "status": "200",
        ///     "data": {
        ///       "id": "123e4567-e89b-12d3-a456-426614174000",
        ///       "parentId": "223e4567-e89b-12d3-a456-426614174001",
        ///       "weight": 65.5,
        ///       "bloodPressure": 120.0
        ///     },
        ///     "message": "Cập nhật ghi chú sức khỏe của mẹ thành công"
        ///   }
        ///   ```
        /// </remarks>
        /// <param name="id">ID của bản ghi sức khỏe của mẹ cần cập nhật.</param>
        /// <param name="request">Thông tin yêu cầu cập nhật sức khỏe. Có thể bao gồm `Weight` và `BloodPressure` (tùy chọn).</param>
        /// <returns>
        /// - `200 OK`: Cập nhật thành công.
        /// - `400 Bad Request`: Yêu cầu không hợp lệ.
        /// - `401 Unauthorized`: Người dùng chưa đăng nhập hoặc không có quyền.
        /// - `404 NotFound`: Bản ghi sức khỏe không tồn tại.
        /// </returns>
        /// <response code="200">Trả về kết quả khi thông tin sức khỏe được cập nhật thành công.</response>
        /// <response code="400">Trả về lỗi nếu yêu cầu không hợp lệ.</response>
        /// <response code="401">Trả về lỗi nếu người dùng chưa đăng nhập hoặc không có quyền.</response>
        /// <response code="404">Trả về lỗi nếu bản ghi sức khỏe không tồn tại.</response>
        [HttpPut(ApiEndPointConstant.MotherHealthRecord.UpdateMotherHealth)]
        [ProducesResponseType(typeof(BaseResponse<UpdateMotherHealthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<UpdateMotherHealthResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<UpdateMotherHealthResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<UpdateMotherHealthResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateMotherHealth([FromRoute] Guid id, [FromBody] UpdateMotherHealthRequest request)
        {
            var response = await _motherHealthService.UpdateMotherHealth(id, request);
            return StatusCode(int.Parse(response.status), response);
        }

        /// <summary>
        /// API lấy thông tin sức khỏe của mẹ theo ID.
        /// </summary>
        /// <remarks>
        /// - API này trả về thông tin sức khỏe của mẹ (cân nặng và huyết áp) dựa trên `id` cung cấp.
        /// - Yêu cầu xác thực (người dùng phải đăng nhập để sử dụng API này).
        /// - Ví dụ yêu cầu:
        ///   ```
        ///   GET /api/v1/mother-health/{id}
        ///   ```
        /// - Kết quả trả về:
        ///   - `200 OK`: Lấy thông tin thành công. Trả về `BaseResponse&lt;GetMotherHealthResponse&gt;` chứa thông tin sức khỏe.
        ///   - `401 Unauthorized`: Người dùng chưa đăng nhập hoặc không có quyền truy cập.
        ///   - `404 NotFound`: Không tìm thấy bản ghi sức khỏe của mẹ với `id` cung cấp.
        /// - Ví dụ phản hồi thành công (200 OK):
        ///   ```json
        ///   {
        ///     "status": "200",
        ///     "data": {
        ///       "weight": 65.5,
        ///       "bloodPressure": 120.0
        ///     },
        ///     "message": "Lấy thông tin sức khỏe của mẹ thành công"
        ///   }
        ///   ```
        /// </remarks>
        /// <param name="id">ID của bản ghi sức khỏe của mẹ cần lấy.</param>
        /// <returns>
        /// - `200 OK`: Lấy thông tin thành công.
        /// - `401 Unauthorized`: Người dùng chưa đăng nhập hoặc không có quyền.
        /// - `404 NotFound`: Bản ghi sức khỏe không tồn tại.
        /// </returns>
        /// <response code="200">Trả về kết quả khi thông tin sức khỏe được lấy thành công.</response>
        /// <response code="401">Trả về lỗi nếu người dùng chưa đăng nhập hoặc không có quyền.</response>
        /// <response code="404">Trả về lỗi nếu bản ghi sức khỏe không tồn tại.</response>
        [HttpGet(ApiEndPointConstant.MotherHealthRecord.GetMotherHealth)]
        [ProducesResponseType(typeof(BaseResponse<GetMotherHealthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetMotherHealthResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<GetMotherHealthResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetMotherHealth([FromRoute] Guid id)
        {
            var response = await _motherHealthService.GetMotherHealth(id);
            return StatusCode(int.Parse(response.status), response);
        }
    }
}
