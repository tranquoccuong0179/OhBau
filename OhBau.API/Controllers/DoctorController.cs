using Microsoft.AspNetCore.Mvc;
using OhBau.Model.Payload.Request.Doctor;
using OhBau.Model.Payload.Request.Major;
using OhBau.Model.Payload.Response.Major;
using OhBau.Model.Payload.Response;
using OhBau.Service.Interface;
using OhBau.Model.Paginate;
using OhBau.Model.Entity;

namespace OhBau.API.Controllers
{
    [ApiController]
    [Route("api/v1/doctor")]
    public class DoctorController(IDoctorService _doctorService, ILogger<DoctorController> _logger) : Controller
    {

        /// <summary>
        /// API tạo chuyên môn mới cho bác sĩ.
        /// </summary>
        /// <remarks>
        /// - API này cho phép tạo một chuyên môn mới bằng cách cung cấp thông tin qua `CreateMajorRequest`.
        /// - Trường `name` trong yêu cầu là bắt buộc.
        /// - API yêu cầu xác thực (JWT) để truy cập.
        /// - API được sử dụng bởi admin.
        /// - Ví dụ yêu cầu:
        ///   ```
        ///   POST /api/v1/doctor/create-major
        ///   ```
        /// - Ví dụ nội dung yêu cầu:
        ///   ```json
        ///   {
        ///     "name": "Cardiology"
        ///   }
        ///   ```
        /// - Kết quả trả về:
        ///   - `200 OK`: Tạo chuyên môn thành công. Trả về `BaseResponse&lt;CreateMajorResponse&gt;` chứa thông tin chuyên môn đã được tạo.
        ///   - `400 Bad Request`: Thông tin đầu vào không hợp lệ (ví dụ: thiếu trường `name` hoặc dữ liệu không đúng định dạng).
        ///   - `401 Unauthorized`: Không cung cấp token hợp lệ hoặc không có quyền truy cập.
        ///   - `500 Internal Server Error`: Lỗi hệ thống trong quá trình xử lý yêu cầu.
        /// - Ví dụ phản hồi thành công (200 OK):
        ///   ```json
        ///   {
        ///     "status": "200",
        ///     "data": {
        ///       "name": "Cardiology"
        ///     },
        ///     "message": "Tạo chuyên môn thành công"
        ///   }
        ///   ```
        /// - Ví dụ phản hồi lỗi (400 Bad Request):
        ///   ```json
        ///   {
        ///     "status": "400",
        ///     "data": null,
        ///     "message": "Thông tin đầu vào không hợp lệ: Major name is required"
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
        /// - Ví dụ phản hồi lỗi (500 Internal Server Error):
        ///   ```json
        ///   {
        ///     "status": "500",
        ///     "data": null,
        ///     "message": "Lỗi hệ thống: [Chi tiết lỗi cụ thể]"
        ///   }
        ///   ```
        /// </remarks>
        /// <param name="request">Thông tin chuyên môn cần tạo. Phải bao gồm `name` (bắt buộc), `active` (mặc định là `true`), và `createAt` (mặc định là thời gian hiện tại).</param>
        /// <returns>
        /// - `200 OK`: Tạo chuyên môn thành công. Trả về `BaseResponse&lt;CreateMajorResponse&gt;` chứa thông tin chuyên môn đã được tạo.
        /// - `400 Bad Request`: Thông tin đầu vào không hợp lệ (ví dụ: thiếu trường `name` hoặc dữ liệu không đúng định dạng).
        /// - `401 Unauthorized`: Không cung cấp token hợp lệ hoặc không có quyền truy cập.
        /// - `500 Internal Server Error`: Lỗi hệ thống trong quá trình xử lý yêu cầu.
        /// </returns>
        /// <response code="200">Trả về thông tin chuyên môn khi tạo thành công.</response>
        /// <response code="400">Trả về lỗi nếu yêu cầu không hợp lệ (ví dụ: thiếu trường `name`).</response>
        /// <response code="401">Trả về lỗi nếu không cung cấp token hợp lệ hoặc không có quyền truy cập.</response>
        /// <response code="500">Trả về lỗi nếu có lỗi hệ thống xảy ra trong quá trình xử lý.</response>
        [HttpPost("create-major")]
        [ProducesResponseType(typeof(BaseResponse<CreateMajorResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CreateMajorResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<CreateMajorResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<CreateMajorResponse>), StatusCodes.Status500InternalServerError)]
        [ProducesErrorResponseType(typeof(BaseResponse<CreateMajorResponse>))]
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

        [HttpPut("edit-doctor-infor/{doctorId}")]
        public async Task<IActionResult> EditDoctorInfor(Guid doctorId, [FromBody]DoctorRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var response = await _doctorService.EditDoctorInfor(doctorId, request);
                return StatusCode(int.Parse(response.status),response);
            }
            catch (Exception ex) {
                _logger.LogError($"[Edit doctor infor] " + ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }

        
        [HttpPut("edit-major/{majorId}")]
        public async Task<IActionResult> EditMajor(Guid majorId, [FromBody]EditMajorRequest request )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _doctorService.EditMajor(majorId, request);
                return StatusCode(int.Parse(response.status), response);
            }
            catch (Exception ex) { 

                _logger.LogError($"[Edit Major] " + ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }


        [HttpPut("DeleteDoctor/{doctorId}")]
        public async Task<IActionResult> DeleteDoctor(Guid doctorId)
        {
            try
            {
                var response = await _doctorService.DeleteDoctor(doctorId);
                return StatusCode(int.Parse(response.status), response);
            }
            catch (Exception ex) {
                _logger.LogError($"[Delete doctor API: ]" + ex.Message,ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPut("delete-major/{majorId}")]
        public async Task<IActionResult>DeleteMajor(Guid majorId)
        {
            try
            {
                var response = await _doctorService.DeleteMajor(majorId);
                return StatusCode(int.Parse(response.status), response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Delete major API: ]" + ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }

        /// <summary>
        /// API lấy danh sách tài khoản người dùng với phân trang.
        /// </summary>
        /// <remarks>
        /// - API này cho phép lấy danh sách tài khoản với hỗ trợ phân trang thông qua các tham số `pageNumber` và `pageSize`.
        /// - Nếu không cung cấp `pageNumber`, mặc định là 1. Nếu không cung cấp `pageSize`, mặc định là 10.
        /// - Ví dụ yêu cầu:
        ///   ```
        ///   GET /api/v1/doctor/get-doctors?pageSize=10&amp;pageNumber=1
        ///   ```
        ///   - Kết quả trả về:
        ///   - `200 OK`: Lấy danh sách tài khoản thành công. Trả về `BaseResponse&lt;IPaginate&lt;GetDoctorsResponse&gt;&gt;` chứa danh sách doctor và thông tin phân trang.
        ///   - `400 Bad Request`: Tham số đầu vào không hợp lệ (ví dụ: `pageNumber` hoặc `pageSize` nhỏ hơn 1).
        ///   - `401 Unauthorized`: Không cung cấp token hợp lệ hoặc không có quyền truy cập.
        ///   - `500 Internal Server Error`: Lỗi hệ thống trong quá trình xử lý yêu cầu.
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
        ///           "fullName": "Doctor 1",
        ///           "address": "123 Main St, City",
        ///           "major": "Cardiology"
        ///         },
        ///         {
        ///           "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///           "fullName": "Doctor 2",
        ///           "address": "123 Main St, City",
        ///           "major": "Cardiology"
        ///         },role": "MOTHER"
        ///         }
        ///       ],
        ///     },
        ///     "message": "Danh sách tài khoản"
        ///   }
        ///   ```
        /// - Ví dụ phản hồi lỗi (400 Bad Request):
        ///   ```json
        ///   {
        ///     "status": "400",
        ///     "data": null,
        ///     "message": "Thông tin đầu vào không hợp lệ: page, size &lt; 1"
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
        /// - Ví dụ phản hồi lỗi (500 Internal Server Error):
        ///   ```json
        ///   {
        ///     "status": "500",
        ///     "data": null,
        ///     "message": "Lỗi hệ thống: [Chi tiết lỗi cụ thể]"
        ///   }
        ///   ```
        /// </remarks>
        /// <param name="pageNumber">Số trang.</param>
        /// <param name="pageSize">Kích thước trang.</param>
        /// <param name="doctorName">Xài kết hợp nếu muốn search bác sĩ theo tên thì truyền tên vào</param>
        /// <returns>
        /// - `200 OK`: Lấy danh sách tài khoản thành công. Trả về `BaseResponse&lt;IPaginate&lt;GetAccountResponse&gt;&gt;` chứa danh sách tài khoản và thông tin phân trang.
        /// - `401 Unauthorized`: Không cung cấp token hợp lệ hoặc không có quyền truy cập.
        /// - `400 Bad Request`: Tham số đầu vào không hợp lệ (ví dụ: `page` hoặc `size` nhỏ hơn 1).
        /// - `500 Internal Server Error`: Lỗi hệ thống trong quá trình xử lý yêu cầu.
        /// </returns>
        /// <response code="200">Trả về danh sách tài khoản và thông tin phân trang khi yêu cầu thành công.</response>
        /// <response code="401">Trả về lỗi nếu không cung cấp token hợp lệ.</response>
        /// <response code="400">Trả về lỗi nếu tham số `page` hoặc `size` không hợp lệ.</response>
        /// <response code="500">Trả về lỗi nếu có lỗi hệ thống xảy ra trong quá trình xử lý.</response>
        [HttpGet("get-doctors")]
        [ProducesResponseType(typeof(BaseResponse<Paginate<GetDoctorsResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<Paginate<GetDoctorsResponse>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<Paginate<GetDoctorsResponse>>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<Paginate<GetDoctorsResponse>>), StatusCodes.Status500InternalServerError)]
        [ProducesErrorResponseType(typeof(BaseResponse<Paginate<GetDoctorsResponse>>))]
        public async Task<IActionResult> GetDoctorWithSearch([FromQuery]int pageSize, [FromQuery]int pageNumber, [FromQuery]string? doctorName)
        {
            try
            {
                var response = await _doctorService.GetDoctors(pageSize, pageNumber,doctorName);
                return StatusCode(int.Parse(response.status), response);
            }
            catch (Exception ex) { 
                
                _logger.LogError(ex, ex.StackTrace);
                return StatusCode(500, ex.ToString() );
            }
        }


        /// <summary>
        /// API lấy thông tin chi tiết của bác sĩ theo ID.
        /// </summary>
        /// <remarks>
        /// - API này trả về thông tin chi tiết của bác sĩ dựa trên `doctorID` được cung cấp.
        /// - Yêu cầu phải cung cấp `doctorID` hợp lệ dưới dạng GUID.
        /// - Ví dụ yêu cầu:
        ///   ```
        ///   GET /api/v1/doctor/get-doctor-infor?doctorID=3fa85f64-5717-4562-b3fc-2c963f66afa6
        ///   ```
        /// - Kết quả trả về:
        ///   - `200 OK`: Lấy thông tin bác sĩ thành công. Trả về `BaseResponse&lt;GetDoctorResponse&gt;` chứa thông tin chi tiết bác sĩ.
        ///   - `400 Bad Request`: `doctorID` không hợp lệ (ví dụ: không phải GUID hoặc rỗng).
        ///   - `401 Unauthorized`: Không cung cấp token hợp lệ hoặc không có quyền truy cập.
        ///   - `404 Not Found`: Không tìm thấy bác sĩ với `doctorID` được cung cấp.
        ///   - `500 Internal Server Error`: Lỗi hệ thống trong quá trình xử lý yêu cầu.
        /// - Ví dụ phản hồi thành công (200 OK):
        ///   ```json
        ///   {
        ///     "status": "200",
        ///     "data": {
        ///       "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///       "fullName": "Doctor 1",
        ///       "major": "Cardiology",
        ///       "dob": "1970-01-01",
        ///       "gender": "Male",
        ///       "content": "Experienced cardiologist",
        ///       "address": "123 Main St, City",
        ///       "email": "doctor1@example.com",
        ///       "phone": "123-456-7890",
        ///       "active": true
        ///     },
        ///     "message": "Lấy thông tin bác sĩ thành công"
        ///   }
        ///   ```
        /// - Ví dụ phản hồi lỗi (400 Bad Request):
        ///   ```json
        ///   {
        ///     "status": "400",
        ///     "data": null,
        ///     "message": "ID bác sĩ không hợp lệ"
        ///   }
        ///   ```
        /// - Ví dụ phản hồi lỗi (404 Not Found):
        ///   ```json
        ///   {
        ///     "status": "404",
        ///     "data": null,
        ///     "message": "Không tìm thấy bác sĩ với ID được cung cấp"
        ///   }
        ///   ```
        /// </remarks>
        /// <param name="doctorID">ID của bác sĩ (GUID).</param>
        /// <returns>
        /// - `200 OK`: Lấy thông tin bác sĩ thành công. Trả về `BaseResponse&lt;GetDoctorResponse&gt;`.
        /// - `400 Bad Request`: `doctorID` không hợp lệ.
        /// - `401 Unauthorized`: Không có quyền truy cập.
        /// - `404 Not Found`: Không tìm thấy bác sĩ.
        /// - `500 Internal Server Error`: Lỗi hệ thống.
        /// </returns>
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
