
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Payload.Request.Parent;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Parent;
using OhBau.Service.Interface;

namespace OhBau.API.Controllers
{
    public class ParentController : BaseController<ParentController>
    {
        private readonly IParentService _parentService;
        public ParentController(ILogger<ParentController> logger, IParentService parentService) : base(logger)
        {
            _parentService = parentService;
        }

        /// <summary>
        /// API tạo mới một bản ghi thông tin phụ huynh.
        /// </summary>
        /// <remarks>
        /// - API này cho phép tạo mới một bản ghi thông tin phụ huynh bằng cách cung cấp thông tin qua `RegisterParentRequest`.
        /// - Các trường trong `RegisterParentRequest` (`FullName` và `Dob`) đều là bắt buộc.
        /// - API yêu cầu xác thực (JWT) để truy cập.
        /// - Ví dụ yêu cầu:
        ///   ```
        ///   POST /api/v1/parent
        ///   ```
        /// - Ví dụ nội dung yêu cầu:
        ///   ```json
        ///   {
        ///     "fullName": "Parent 1",
        ///     "dob": "1990-05-01"
        ///   }
        ///   ```
        /// - Kết quả trả về:
        ///   - `200 OK`: Tạo phụ huynh thành công. Trả về `BaseResponse&lt;RegisterParentResponse&gt;` chứa thông tin phụ huynh đã được tạo.
        ///   - `400 Bad Request`: Thông tin đầu vào không hợp lệ (ví dụ: thiếu trường bắt buộc hoặc dữ liệu không đúng định dạng).
        /// - Ví dụ phản hồi thành công (200 OK):
        ///   ```json
        ///   {
        ///     "status": "200",
        ///     "data": {
        ///       "fullName": "Parent 1",
        ///       "dob": "1990-05-01"
        ///     },
        ///     "message": "Tạo thông tin phụ huynh thành công"
        ///   }
        ///   ```
        /// - Ví dụ phản hồi lỗi (400 Bad Request):
        ///   ```json
        ///   {
        ///     "status": "400",
        ///     "data": null,
        ///     "message": "Dữ liệu đầu vào không hợp lệ",
        ///     "errors": {
        ///       "FullName": ["Họ và tên không được để trống"]
        ///     }
        ///   }
        ///   ```
        /// - Ví dụ phản hồi lỗi (401 Unauthorized):
        ///   ```json
        ///   {
        ///     "status": "401",
        ///     "data": null,
        ///     "message": "Không cung cấp token hợp lệ hoặc không có quyền truy cập"
        ///   }
        ///   ```
        /// </remarks>
        /// <param name="request">Thông tin phụ huynh cần tạo. Bao gồm các trường `fullName` và `dob` (đều bắt buộc).</param>
        /// <returns>
        /// - `200 OK`: Tạo phụ huynh thành công.
        /// - `400 Bad Request`: Thông tin đầu vào không hợp lệ.
        /// - `401 Unauthorized`: Không cung cấp token hợp lệ hoặc không có quyền truy cập.
        /// </returns>
        /// <response code="200">Trả về thông tin phụ huynh khi tạo thành công.</response>
        /// <response code="400">Trả về lỗi nếu thông tin đầu vào không hợp lệ.</response>
        /// <response code="401">Trả về lỗi nếu không cung cấp token hợp lệ.</response>
        [HttpPost(ApiEndPointConstant.Parent.CreateParent)]
        [ProducesResponseType(typeof(BaseResponse<RegisterParentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<RegisterParentResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<RegisterParentResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> CreateParent([FromBody] RegisterParentRequest request) 
        {
            var response = await _parentService.AddNewParent(request);
            return StatusCode(int.Parse(response.status), response);
        }
    }
}
