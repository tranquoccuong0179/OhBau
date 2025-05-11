
using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.ParentRelation;
using OhBau.Service.Implement;
using OhBau.Service.Interface;

namespace OhBau.API.Controllers
{
    public class ParentRelationController : BaseController<ParentRelationController>
    {
        private readonly IParentRelationService _parentRelationService;
        public ParentRelationController(ILogger<ParentRelationController> logger, IParentRelationService parentRelationService) : base(logger)
        {
            _parentRelationService = parentRelationService;
        }

        /// <summary>
        /// API lấy thông tin mối quan hệ phụ huynh và thai nhi.
        /// </summary>
        /// <remarks>
        /// - API này cho phép lấy thông tin chi tiết về mối quan hệ phụ huynh (bố, mẹ) và danh sách thai nhi liên quan.
        /// - API yêu cầu xác thực (JWT) để truy cập.
        /// - Kết quả trả về khác nhau tùy thuộc vào vai trò của người dùng:
        ///   - Nếu người dùng là **bố**, thông tin cả bố và mẹ (bao gồm `getMotherHealthResponse`) được trả về.
        ///   - Nếu người dùng là **mẹ**, chỉ thông tin mẹ (bao gồm `getMotherHealthResponse`) được trả về, thông tin bố là `null`.
        /// - Ví dụ yêu cầu:
        ///   ```
        ///   GET /api/v1/parent-relation
        ///   ```
        /// - Kết quả trả về:
        ///   - `200 OK`: Lấy thông tin mối quan hệ thành công. Trả về `BaseResponse&lt;GetParentRelationResponse&gt;` chứa thông tin phụ huynh và thai nhi.
        ///   - `404 Not Found`: Không tìm thấy thông tin mối quan hệ phụ huynh.
        /// - Ví dụ phản hồi thành công (200 OK) khi người dùng là **Father**:
        ///   ```json
        ///   {
        ///     "status": "200",
        ///     "message": "Lấy thông tin mối quan hệ phụ huynh thành công",
        ///     "data": {
        ///       "father": {
        ///         "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "fullName": "Father 1",
        ///         "dob": "1990-05-01",
        ///         "getMotherHealthResponse": null
        ///       },
        ///       "mother": {
        ///         "id": "d826a39b-fe01-4b55-a438-b13b33ff59f2",
        ///         "fullName": "Mother 1",
        ///         "dob": "1992-06-15",
        ///         "getMotherHealthResponse": {
        ///           "weight": 60.5,
        ///           "bloodPressure": 120
        ///         }
        ///       },
        ///       "fetuses": [
        ///         {
        ///           "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
        ///           "startDate": "2025-01-01",
        ///           "endDate": "2025-09-30",
        ///           "name": "Baby 1",
        ///           "code": "FETUS123456",
        ///           "weight": 3.5,
        ///           "height": 50.0
        ///         }
        ///       ]
        ///     }
        ///   }
        ///   ```
        /// - Ví dụ phản hồi thành công (200 OK) khi người dùng là **Mother**:
        ///   ```json
        ///   {
        ///     "status": "200",
        ///     "message": "Lấy thông tin mối quan hệ phụ huynh thành công",
        ///     "data": {
        ///       "father": null,
        ///       "mother": {
        ///         "id": "d826a39b-fe01-4b55-a438-b13b33ff59f2",
        ///         "fullName": "Mother 1",
        ///         "dob": "1992-06-15",
        ///         "getMotherHealthResponse": {
        ///           "weight": 60.5,
        ///           "bloodPressure": 120
        ///         }
        ///       },
        ///       "fetuses": [
        ///         {
        ///           "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
        ///           "startDate": "2025-01-01",
        ///           "endDate": "2025-09-30",
        ///           "name": "Baby 1",
        ///           "code": "FETUS123456",
        ///           "weight": 3.5,
        ///           "height": 50.0
        ///         }
        ///       ]
        ///     }
        ///   }
        ///   ```
        /// - Ví dụ phản hồi lỗi (404 Not Found):
        ///   ```json
        ///   {
        ///     "status": "404",
        ///     "data": null,
        ///     "message": "Không tìm thấy thông tin mối quan hệ phụ huynh"
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
        /// <returns>
        /// - `200 OK`: Lấy thông tin mối quan hệ phụ huynh thành công.
        /// - `404 Not Found`: Không tìm thấy thông tin mối quan hệ phụ huynh.
        /// - `401 Unauthorized`: Không cung cấp token hợp lệ hoặc không có quyền truy cập.
        /// </returns>
        /// <response code="200">Trả về thông tin mối quan hệ phụ huynh và thai nhi khi yêu cầu thành công.</response>
        /// <response code="404">Trả về lỗi nếu không tìm thấy thông tin mối quan hệ phụ huynh.</response>
        /// <response code="401">Trả về lỗi nếu không cung cấp token hợp lệ.</response>
        [HttpGet(ApiEndPointConstant.ParentRelation.GetParentRelation)]
        [ProducesResponseType(typeof(BaseResponse<GetParentRelationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetParentRelationResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<GetParentRelationResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetParentRelation()
        {
            var response = await _parentRelationService.GetParentRelation();
            return StatusCode(int.Parse(response.status), response);
        }
    }
}
