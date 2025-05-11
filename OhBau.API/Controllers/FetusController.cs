using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Exception;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Request.Fetus;
using OhBau.Model.Payload.Response.Fetus;
using OhBau.Service.Interface;
using OhBau.Model.Paginate;

namespace OhBau.API.Controllers
{
    public class FetusController : BaseController<FetusController>
    {
        private readonly IFetusService _fetusService;
        private readonly IDoctorService _doctorService;
        public FetusController(ILogger<FetusController> logger, IFetusService fetusService, IDoctorService doctorService) : base(logger)
        {
            _fetusService = fetusService;
            _doctorService = doctorService;
        }


        /// <summary>
        /// API tạo mới một bản ghi thông tin thai nhi.
        /// </summary>
        /// <remarks>
        /// - API này cho phép tạo mới một bản ghi thông tin thai nhi bằng cách cung cấp thông tin qua `CreateFetusRequest`.
        /// - Các trường trong `CreateFetusRequest` đều là tùy chọn, nhưng cần đảm bảo dữ liệu hợp lệ khi gửi yêu cầu.
        /// - API yêu cầu xác thực (JWT) để truy cập và thêm hồ sơ thai nhi vào `ParentRelation`.
        /// - Ví dụ yêu cầu:
        ///   ```
        ///   POST /api/v1/fetus
        ///   ```
        /// - Ví dụ nội dung yêu cầu:
        ///   ```json
        ///   {
        ///     "startDate": "2025-01-01",
        ///     "endDate": "2025-09-30",
        ///     "name": "Baby 1",
        ///     "weight": 3.5,
        ///     "height": 50.0
        ///   }
        ///   ```
        /// - Kết quả trả về:
        ///   - `200 OK`: Tạo thai nhi thành công. Trả về `BaseResponse&lt;CreateFetusResponse&gt;` chứa thông tin thai nhi đã được tạo.
        ///   - `400 Bad Request`: Thông tin đầu vào không hợp lệ (ví dụ: dữ liệu không đúng định dạng).
        /// - Ví dụ phản hồi thành công (200 OK):
        ///   ```json
        ///   {
        ///     "status": "200",
        ///     "data": {
        ///       "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///       "startDate": "2025-01-01",
        ///       "endDate": "2025-09-30",
        ///       "name": "Baby 1",
        ///       "code": "FETUS12345",
        ///       "weight": 3.5,
        ///       "height": 50.0
        ///     },
        ///     "message": "Tạo thai nhi thành công"
        ///   }
        ///   ```
        /// - Ví dụ phản hồi lỗi (400 Bad Request):
        ///   ```json
        ///   {
        ///     "status": "400",
        ///     "data": null,
        ///     "message": "Dữ liệu đầu vào không hợp lệ",
        ///     "errors": {
        ///       "Weight": ["Cân nặng phải lớn hơn 0"]
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
        /// <param name="request">Thông tin thai nhi cần tạo. Bao gồm các trường như `startDate`, `endDate`, `name`, `weight`, `height` (tất cả đều tùy chọn).</param>
        /// <returns>
        /// - `200 OK`: Tạo thai nhi thành công.
        /// - `400 Bad Request`: Thông tin đầu vào không hợp lệ.
        /// - `401 Unauthorized`: Không cung cấp token hợp lệ hoặc không có quyền truy cập.
        /// </returns>
        /// <response code="200">Trả về thông tin thai nhi khi tạo thành công.</response>
        /// <response code="400">Trả về lỗi nếu thông tin đầu vào không hợp lệ.</response>
        /// <response code="401">Trả về lỗi nếu không cung cấp token hợp lệ.</response>
        [HttpPost(ApiEndPointConstant.Fetus.CreateFetus)]
        [ProducesResponseType(typeof(BaseResponse<CreateFetusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CreateFetusResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<CreateFetusResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> CreateFetus([FromBody] CreateFetusRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );
                throw new ModelValidationException(errors);
            }
            var response = await _fetusService.CreateFetus(request);
            return StatusCode(int.Parse(response.status), response);
        }

        /// <summary>
        /// API lấy danh sách thai nhi với phân trang.
        /// </summary>
        /// <remarks>
        /// - API này cho phép lấy danh sách tài khoản với hỗ trợ phân trang thông qua các tham số `page` và `size`.
        /// - Nếu không cung cấp `page`, mặc định là 1. Nếu không cung cấp `size`, mặc định là 10.
        /// - API yêu cầu xác thực (JWT) để truy cập.
        /// - API được sử dụng bởi `ADMIN`
        /// - API dùng cho doctor
        /// - Ví dụ yêu cầu:
        ///   ```
        ///   GET /api/v1/fetus?page=1&amp;size=10
        ///   ```
        ///   - Kết quả trả về:
        ///   - `200 OK`: Lấy danh sách tài khoản thành công. Trả về `BaseResponse&lt;IPaginate&lt;GetFetusResponse&gt;&gt;` chứa danh sách tài khoản và thông tin phân trang.
        ///   - `400 Bad Request`: Tham số đầu vào không hợp lệ (ví dụ: `page` hoặc `size` nhỏ hơn 1).
        ///   - `401 Unauthorized`: Không cung cấp token hợp lệ hoặc không có quyền truy cập.
        /// - Ví dụ phản hồi thành công (200 OK):
        ///   ```json
        ///   {
        ///     "status": "200",
        ///     "data": {
        ///       "size": 10,
        ///       "page": 1,
        ///       "total": 2,
        ///       "totalPages": 1,
        ///       "items": [
        ///         {
        ///           "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///           "startDate": "2025-01-01",
        ///           "endDate": "2025-09-30",
        ///           "name": "Baby 1",
        ///           "code": "FETUS123456",
        ///           "weight": 3.5,
        ///           "height": 50.0
        ///         }
        ///       ],
        ///     },
        ///     "message": "Danh sách thai nhi"
        ///   }
        ///   ```
        /// </remarks>
        /// <param name="page">Số trang (tùy chọn, mặc định là 1).</param>
        /// <param name="size">Kích thước trang (tùy chọn, mặc định là 10).</param>
        /// <returns>
        /// - `200 OK`: Lấy danh sách tài khoản thành công. Trả về `BaseResponse&lt;IPaginate&lt;GetFetusResponse&gt;&gt;` chứa danh sách tài khoản và thông tin phân trang.
        /// - `401 Unauthorized`: Không cung cấp token hợp lệ hoặc không có quyền truy cập.
        /// - `400 Bad Request`: Tham số đầu vào không hợp lệ (ví dụ: `page` hoặc `size` nhỏ hơn 1).
        /// </returns>
        /// <response code="200">Trả về danh sách thai nhi và phân trang khi yêu cầu thành công.</response>
        /// <response code="401">Trả về lỗi nếu không cung cấp token hợp lệ.</response>
        /// <response code="400">Trả về lỗi nếu tham số `page` hoặc `size` không hợp lệ.</response>
        [HttpGet(ApiEndPointConstant.Fetus.GetAllFetus)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<GetFetusResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<GetFetusResponse>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<GetFetusResponse>>), StatusCodes.Status401Unauthorized)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetAllFetus([FromQuery] int? page, [FromQuery] int? size)
        {
            int pageNumber = page ?? 1;
            int pageSize = size ?? 10;
            var response = await _fetusService.GetAllFetus(pageNumber, pageSize);
            return StatusCode(int.Parse(response.status), response);
        }

        /// <summary>
        /// API lấy thông tin chi tiết của một thai nhi dựa trên ID.
        /// </summary>
        /// <remarks>
        /// - API này cho phép lấy thông tin chi tiết của một thai nhi bằng cách cung cấp `id` qua đường dẫn (route).
        /// - API yêu cầu xác thực (JWT) để truy cập.
        /// - Ví dụ yêu cầu:
        ///   ```
        ///   GET /api/v1/fetus/{id}
        ///   ```
        ///   Ví dụ: `GET /api/v1/fetus/3fa85f64-5717-4562-b3fc-2c963f66afa6`
        /// - Kết quả trả về:
        ///   - `200 OK`: Lấy thông tin thai nhi thành công. Trả về `BaseResponse&lt;GetFetusResponse&gt;` chứa thông tin chi tiết.
        ///   - `404 Not Found`: Không tìm thấy thai nhi với ID đã cung cấp.
        /// - Ví dụ phản hồi thành công (200 OK):
        ///   ```json
        ///   {
        ///     "status": "200",
        ///     "data": {
        ///       "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///       "startDate": "2025-01-01",
        ///       "endDate": "2025-09-30",
        ///       "name": "Baby 1",
        ///       "code": "FETUS123456",
        ///       "weight": 3.5,
        ///       "height": 50.0
        ///     },
        ///     "message": "Lấy thông tin thai nhi thành công"
        ///   }
        ///   ```
        /// - Ví dụ phản hồi lỗi (404 Not Found):
        ///   ```json
        ///   {
        ///     "status": "404",
        ///     "data": null,
        ///     "message": "Không tìm thấy thai nhi với ID này"
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
        /// <param name="id">ID của thai nhi cần lấy thông tin (định dạng GUID).</param>
        /// <returns>
        /// - `200 OK`: Lấy thông tin thai nhi thành công.
        /// - `404 Not Found`: Không tìm thấy thai nhi với ID đã cung cấp.
        /// - `401 Unauthorized`: Không cung cấp token hợp lệ hoặc không có quyền truy cập.
        /// </returns>
        /// <response code="200">Trả về thông tin chi tiết của thai nhi khi yêu cầu thành công.</response>
        /// <response code="404">Trả về lỗi nếu không tìm thấy thai nhi với ID đã cung cấp.</response>
        /// <response code="401">Trả về lỗi nếu không cung cấp token hợp lệ.</response>
        [HttpGet(ApiEndPointConstant.Fetus.GetFetusById)]
        [ProducesResponseType(typeof(BaseResponse<GetFetusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetFetusResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<GetFetusResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _fetusService.GetFetusById(id);
            return StatusCode(int.Parse(response.status), response);
        }

        /// <summary>
        /// API lấy thông tin chi tiết của một thai nhi dựa trên mã (code).
        /// </summary>
        /// <remarks>
        /// - API này cho phép lấy thông tin chi tiết của một thai nhi bằng cách cung cấp `code` qua query parameter.
        /// - API yêu cầu xác thực (JWT) để truy cập.
        /// - Ví dụ yêu cầu:
        ///   ```
        ///   GET /api/v1/fetus/code?code=FETUS123456
        ///   ```
        /// - Kết quả trả về:
        ///   - `200 OK`: Lấy thông tin thai nhi thành công. Trả về `BaseResponse&lt;GetFetusResponse&gt;` chứa thông tin chi tiết.
        ///   - `404 Not Found`: Không tìm thấy thai nhi với mã đã cung cấp.
        /// - Ví dụ phản hồi thành công (200 OK):
        ///   ```json
        ///   {
        ///     "status": "200",
        ///     "data": {
        ///       "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///       "startDate": "2025-01-01",
        ///       "endDate": "2025-09-30",
        ///       "name": "Baby 1",
        ///       "code": "FETUS123456",
        ///       "weight": 3.5,
        ///       "height": 50.0
        ///     },
        ///     "message": "Lấy thông tin thai nhi thành công"
        ///   }
        ///   ```
        /// - Ví dụ phản hồi lỗi (404 Not Found):
        ///   ```json
        ///   {
        ///     "status": "404",
        ///     "data": null,
        ///     "message": "Không tìm thấy thai nhi với mã này"
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
        /// <param name="code">Mã của thai nhi cần lấy thông tin.</param>
        /// <returns>
        /// - `200 OK`: Lấy thông tin thai nhi thành công.
        /// - `404 Not Found`: Không tìm thấy thai nhi với mã đã cung cấp.
        /// - `401 Unauthorized`: Không cung cấp token hợp lệ hoặc không có quyền truy cập.
        /// </returns>
        /// <response code="200">Trả về thông tin chi tiết của thai nhi khi yêu cầu thành công.</response>
        /// <response code="404">Trả về lỗi nếu không tìm thấy thai nhi với mã đã cung cấp.</response>
        /// <response code="401">Trả về lỗi nếu không cung cấp token hợp lệ.</response>
        [HttpGet(ApiEndPointConstant.Fetus.GetFetusByCode)]
        [ProducesResponseType(typeof(BaseResponse<GetFetusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetFetusResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<GetFetusResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetByCode([FromQuery] string code)
        {
            var response = await _fetusService.GetFetusByCode(code);
            return StatusCode(int.Parse(response.status), response);
        }

        [HttpPut(ApiEndPointConstant.Fetus.UpdateFetus)]
        [ProducesResponseType(typeof(BaseResponse<GetFetusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetFetusResponse>), StatusCodes.Status404NotFound)]
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
