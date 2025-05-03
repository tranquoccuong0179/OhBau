
using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Exception;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Account;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Account;
using OhBau.Model.Payload.Response.Parent;
using OhBau.Service.Interface;

namespace OhBau.API.Controllers
{
    public class AccountController : BaseController<AccountController>
    {
        private readonly IAccountService _accountService;
        public AccountController(ILogger<AccountController> logger, IAccountService accountService) : base(logger)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// API đăng ký tài khoản mới cho người dùng.
        /// </summary>
        /// <remarks>
        /// - API này cho phép tạo tài khoản mới bằng cách cung cấp thông tin qua `RegisterRequest`.
        /// - Trường `role` trong yêu cầu phải là một trong các giá trị sau: `FATHER` hoặc `MOTHER`.
        /// - Tất cả các trường trong `RegisterRequest` đều bắt buộc.
        /// - Không yêu cầu xác thực (public API).
        /// - Ví dụ yêu cầu:
        ///   ```
        ///   POST /api/v1/account
        ///   ```
        /// - Ví dụ nội dung yêu cầu:
        ///   ```json
        ///   {
        ///     "phone": "0987654321",
        ///     "email": "cuongtq@gmail.com",
        ///     "password": "123456",
        ///     "role": "FATHER/MOTHER",
        ///     "registerParentRequest": {
        ///       "fullName": "Father 1",
        ///       "dob": "1990-05-01"
        ///     }
        ///    }
        ///    ```
        /// - Kết quả trả về:
        ///   - `200 OK`: Cập nhật tài khoản thành công. Trả về `BaseResponse&lt;RegisterResponse&gt;` chứa thông tin tài khoản đã được cập nhật.
        ///   - `400 Bad Request`: Thông tin đầu vào không hợp lệ (ví dụ: thiếu trường hoặc dữ liệu không đúng định dạng).
        /// - Ví dụ phản hồi thành công (200 OK):
        ///   ```json
        ///   {
        ///     "status": "200",
        ///     "data": {
        ///       "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///       "email": "cuongtq@gmail.com",
        ///       "phone": "0987654321",
        ///       "role": "FATHER",
        ///       "registerParentRequest": {
        ///         "fullName": "Father 1",
        ///         "dob": "1990-05-01"
        ///        }
        ///     },
        ///     "message": "Đăng kí tài khoản thành công"
        ///   }
        ///   ```
        /// </remarks>
        /// <param name="request">Thông tin đăng ký của người dùng. Phải bao gồm `phone`, `email`, `password`, và `role` (giá trị cho phép: `FATHER`, `MOTHER`).</param>
        /// <returns>
        /// - `200 OK`: Đăng ký thành công.  
        /// - `400 Bad Request`: Thông tin đầu vào không hợp lệ (ví dụ: thiếu trường, email không hợp lệ, hoặc `role` không được hỗ trợ).
        /// </returns>
        /// <response code="200">Trả về kết quả đăng ký khi tài khoản được tạo thành công.</response>
        /// <response code="400">Trả về lỗi nếu yêu cầu không hợp lệ hoặc `role` không phải là `FATHER`, `MOTHER`.</response>
        [HttpPost(ApiEndPointConstant.Account.RegisterAccount)]
        [ProducesResponseType(typeof(BaseResponse<RegisterResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<RegisterResponse>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> CreateNewAccount([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );
                throw new ModelValidationException(errors);
            }
            var response = await _accountService.RegisterAccount(request);
            return StatusCode(int.Parse(response.status), response);
        }

        /// <summary>
        /// API lấy danh sách tài khoản người dùng với phân trang.
        /// </summary>
        /// <remarks>
        /// - API này cho phép lấy danh sách tài khoản với hỗ trợ phân trang thông qua các tham số `page` và `size`.
        /// - Nếu không cung cấp `page`, mặc định là 1. Nếu không cung cấp `size`, mặc định là 10.
        /// - API yêu cầu xác thực (JWT) để truy cập.
        /// - API được sử dụng bởi `ADMIN`
        /// - Ví dụ yêu cầu:
        ///   ```
        ///   GET /api/v1/account?page=1&amp;size=10
        ///   ```
        ///   - Kết quả trả về:
        ///   - `200 OK`: Lấy danh sách tài khoản thành công. Trả về `BaseResponse&lt;IPaginate&lt;GetAccountResponse&gt;&gt;` chứa danh sách tài khoản và thông tin phân trang.
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
        ///           "id": "d826a39b-fe01-4b55-a438-b13b33ff59f2",
        ///           "email": "cuongtq@gmail.com",
        ///           "phone": "0987654321",
        ///           "role": "FATHER"
        ///         },
        ///         {
        ///           "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///           "email": "user2@example.com",
        ///           "phone": "0987654322",
        ///           "role": "MOTHER"
        ///         }
        ///       ],
        ///     },
        ///     "message": "Danh sách tài khoản"
        ///   }
        ///   ```
        /// </remarks>
        /// <param name="page">Số trang (tùy chọn, mặc định là 1).</param>
        /// <param name="size">Kích thước trang (tùy chọn, mặc định là 10).</param>
        /// <returns>
        /// - `200 OK`: Lấy danh sách tài khoản thành công. Trả về `BaseResponse&lt;IPaginate&lt;GetAccountResponse&gt;&gt;` chứa danh sách tài khoản và thông tin phân trang.
        /// - `401 Unauthorized`: Không cung cấp token hợp lệ hoặc không có quyền truy cập.
        /// - `400 Bad Request`: Tham số đầu vào không hợp lệ (ví dụ: `page` hoặc `size` nhỏ hơn 1).
        /// </returns>
        /// <response code="200">Trả về danh sách tài khoản và thông tin phân trang khi yêu cầu thành công.</response>
        /// <response code="401">Trả về lỗi nếu không cung cấp token hợp lệ.</response>
        /// <response code="400">Trả về lỗi nếu tham số `page` hoặc `size` không hợp lệ.</response>
        [HttpGet(ApiEndPointConstant.Account.GetAccounts)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<GetAccountResponse>>), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetAccounts([FromQuery] int? page, [FromQuery] int? size)
        {
            int pageNumber = page ?? 1;
            int pageSize = size ?? 10;
            var response = await _accountService.GetAccounts(pageNumber, pageSize);
            return StatusCode(int.Parse(response.status), response);
        }

        /// <summary>
        /// API lấy thông tin chi tiết của một tài khoản dựa trên ID.
        /// </summary>
        /// <remarks>
        /// - API này cho phép lấy thông tin chi tiết của một tài khoản bằng cách cung cấp `id` qua đường dẫn (route).
        /// - Yêu cầu xác thực: Người dùng phải cung cấp token hợp lệ trong header `Authorization`.
        /// - API yêu cầu xác thực (JWT) để truy cập.
        /// - API được sử dụng bởi `ADMIN`
        /// - Ví dụ yêu cầu:
        ///   ```
        ///   GET /api/v1/account/{id}
        ///   ```
        ///   Ví dụ: `GET /api/v1/account/d826a39b-fe01-4b55-a438-b13b33ff59f2`
        /// - Kết quả trả về:
        ///   - `200 OK`: Cập nhật tài khoản thành công. Trả về `BaseResponse&lt;GetAccountResponse&gt;` chứa thông tin tài khoản đã được cập nhật.
        ///   - `404 Not Found`: Không tìm thấy tài khoản cần cập nhật.
        ///   - Ví dụ phản hồi thành công (200 OK):
        ///   ```json
        ///   {
        ///     "status": "200",
        ///     "data": {
        ///       "Id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///       "Email": "cuongtq@gmail.com",
        ///       "Phone": "0987654321",
        ///       "Role": "FATHER"
        ///     },
        ///     "message": "Cập nhật tài khoản thành công"
        ///   }
        ///   ```
        /// </remarks>
        /// <param name="id">ID của tài khoản cần lấy thông tin (định dạng GUID).</param>
        /// <returns>
        /// - `200 OK`: Lấy thông tin tài khoản thành công. Trả về `BaseResponse&lt;GetAccountResponse;&gt;` chứa thông tin chi tiết của tài khoản.
        /// - `404 Not Found`: Không tìm thấy tài khoản với ID đã cung cấp.
        /// - `401 Unauthorized`: Không cung cấp token hợp lệ hoặc không có quyền truy cập.
        /// </returns>
        /// <response code="200">Trả về thông tin chi tiết của tài khoản khi yêu cầu thành công.</response>
        /// <response code="404">Trả về lỗi nếu không tìm thấy tài khoản với ID đã cung cấp.</response>
        /// <response code="401">Trả về lỗi nếu không cung cấp token hợp lệ.</response>
        [HttpGet(ApiEndPointConstant.Account.GetAccount)]
        [ProducesResponseType(typeof(BaseResponse<GetAccountResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetAccountResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetAccountById([FromRoute] Guid id)
        {
            var response = await _accountService.GetAccount(id);
            return StatusCode(int.Parse(response.status), response);
        }

        /// <summary>
        /// API lấy thông tin hồ sơ tài khoản của người dùng hiện tại.
        /// </summary>
        /// <remarks>
        /// - API này cho phép lấy thông tin chi tiết của hồ sơ tài khoản hiện tại của người dùng đã xác thực.
        /// - Yêu cầu xác thực: Người dùng phải cung cấp token hợp lệ trong header `Authorization`.
        /// - API yêu cầu xác thực (JWT) để truy cập.
        /// - API được sử dụng bởi người dùng đã đăng nhập.
        /// - Ví dụ yêu cầu:
        ///   ```
        ///   GET /api/v1/account/profile
        ///   ```
        /// - Kết quả trả về:
        ///   - `200 OK`: Lấy thông tin hồ sơ tài khoản thành công. Trả về `BaseResponse&lt;GetParentResponse&gt;` chứa thông tin tài khoản.
        ///   - `404 Not Found`: Không tìm thấy thông tin tài khoản của người dùng hiện tại.
        ///   - `401 Unauthorized`: Không cung cấp token hợp lệ hoặc không có quyền truy cập.
        /// - Ví dụ phản hồi thành công (200 OK):
        ///   ```json
        ///   {
        ///     "status": "200",
        ///     "data": {
        ///       "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///       "fullName": "Father 1",
        ///       "dob": "1990-05-01"
        ///     },
        ///     "message": "Lấy thông tin hồ sơ tài khoản thành công"
        ///   }
        ///   ```
        /// - Ví dụ phản hồi lỗi (404 Not Found):
        ///   ```json
        ///   {
        ///     "status": "404",
        ///     "data": null,
        ///     "message": "Không tìm thấy thông tin tài khoản của người dùng hiện tại"
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
        /// - `200 OK`: Lấy thông tin hồ sơ tài khoản thành công. Trả về `BaseResponse&lt;GetParentResponse&gt;` chứa thông tin chi tiết của tài khoản.
        /// - `404 Not Found`: Không tìm thấy thông tin tài khoản của người dùng hiện tại.
        /// - `401 Unauthorized`: Không cung cấp token hợp lệ hoặc không có quyền truy cập.
        /// </returns>
        /// <response code="200">Trả về thông tin hồ sơ tài khoản khi yêu cầu thành công.</response>
        /// <response code="404">Trả về lỗi nếu không tìm thấy thông tin tài khoản của người dùng hiện tại.</response>
        /// <response code="401">Trả về lỗi nếu không cung cấp token hợp lệ hoặc không có quyền truy cập.</response>
        [HttpGet(ApiEndPointConstant.Account.GetAccountProfile)]
        [ProducesResponseType(typeof(BaseResponse<GetParentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetParentResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<GetParentResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetAccountProfile()
        {
            var response = await _accountService.GetAccountProfile();
            return StatusCode(int.Parse(response.status), response);
        }

        /// <summary>
        /// API cập nhật thông tin tài khoản người dùng.
        /// </summary>
        /// <remarks>
        /// - API này cho phép cập nhật thông tin tài khoản người dùng thông qua `UpdateAccountRequest`.
        /// - Yêu cầu xác thực: Người dùng phải cung cấp token hợp lệ trong header `Authorization`.
        /// - API yêu cầu xác thực (JWT) để truy cập.
        /// - Ví dụ yêu cầu:
        ///   ```
        ///   PUT /api/v1/account
        ///   ```
        /// - Ví dụ nội dung yêu cầu:
        ///   ```json
        ///   {
        ///     "phone": "0987654321"
        ///   }
        ///   ```
        /// - Kết quả trả về:
        ///   - `200 OK`: Cập nhật tài khoản thành công. Trả về `BaseResponse&lt;GetAccountResponse&gt;` chứa thông tin tài khoản đã được cập nhật.
        ///   - `400 Bad Request`: Thông tin đầu vào không hợp lệ (ví dụ: thiếu trường hoặc dữ liệu không đúng định dạng).
        ///   - `404 Not Found`: Không tìm thấy tài khoản cần cập nhật.
        /// - Ví dụ phản hồi thành công (200 OK):
        ///   ```json
        ///   {
        ///     "status": "200",
        ///     "data": {
        ///       "Id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///       "Email": "cuongtq@gmail.com",
        ///       "Phone": "0987654321",
        ///       "Role": "FATHER"
        ///     },
        ///     "message": "Cập nhật tài khoản thành công"
        ///   }
        ///   ```
        /// </remarks>
        /// <param name="request">Thông tin cập nhật tài khoản. Phải bao gồm `accountId` và các trường cần cập nhật (như `fullName`, `email`, `phone`).</param>
        /// <returns>
        /// - `200 OK`: Cập nhật tài khoản thành công. Trả về `BaseResponse&lt;GetAccountResponse&gt;` chứa thông tin tài khoản đã được cập nhật.
        /// - `400 Bad Request`: Thông tin đầu vào không hợp lệ (ví dụ: thiếu trường hoặc dữ liệu không đúng định dạng).
        /// - `404 Not Found`: Không tìm thấy tài khoản cần cập nhật.
        /// </returns>
        /// <response code="200">Cập nhật tài khoản thành công. Trả về thông tin tài khoản đã được cập nhật.</response>
        /// <response code="400">Trả về lỗi nếu thông tin đầu vào không hợp lệ (ví dụ: thiếu trường hoặc dữ liệu không đúng định dạng).</response>
        /// <response code="404">Trả về lỗi nếu không tìm thấy tài khoản cần cập nhật.</response>
        [HttpPut(ApiEndPointConstant.Account.UpdateAccount)]
        [ProducesResponseType(typeof(BaseResponse<GetAccountResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetAccountResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<GetAccountResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountRequest request)
        {
            var response = await _accountService.UpdateAccount(request);
            return StatusCode(int.Parse(response.status), response);
        }

        /// <summary>
        /// API xóa tài khoản người dùng dựa trên ID.
        /// </summary>
        /// <remarks>
        /// - API này cho phép xóa một tài khoản người dùng bằng cách cung cấp `id` qua đường dẫn (route).
        /// - Yêu cầu xác thực: Người dùng phải cung cấp token hợp lệ trong header `Authorization`.
        /// - API yêu cầu xác thực (JWT) để truy cập.
        /// - API được sử dụng bởi admin.
        /// - Ví dụ yêu cầu:
        ///   ```
        ///   DELETE /api/v1/account/{id}
        ///   ```
        ///   Ví dụ: `DELETE /api/v1/account/3fa85f64-5717-4562-b3fc-2c963f66afa6`
        /// - Kết quả trả về:
        ///   - `200 OK`: Xóa tài khoản thành công. Trả về `BaseResponse&lt;bool&gt;` với giá trị `true`.
        ///   - `404 Not Found`: Không tìm thấy tài khoản với ID đã cung cấp.
        ///   - `401 Unauthorized`: Không cung cấp token hợp lệ hoặc không có quyền truy cập.
        /// - Ví dụ phản hồi thành công (200 OK):
        ///   ```json
        ///   {
        ///     "status": "200",
        ///     "data": true,
        ///     "message": "Xóa tài khoản thành công"
        ///   }
        ///   ```
        /// - Ví dụ phản hồi lỗi (404 Not Found):
        ///   ```json
        ///   {
        ///     "status": "404",
        ///     "data": false,
        ///     "message": "Không tìm thấy tài khoản này"
        ///   }
        ///   ```
        /// - Ví dụ phản hồi lỗi (401 Unauthorized):
        ///   ```json
        ///   {
        ///     "status": "401",
        ///     "data": false,
        ///     "message": "Không cung cấp token hợp lệ hoặc không có quyền truy cập"
        ///   }
        ///   ```
        /// </remarks>
        /// <param name="id">ID của tài khoản cần xóa (định dạng GUID).</param>
        /// <returns>
        /// - `200 OK`: Xóa tài khoản thành công. Trả về `BaseResponse&lt;bool&gt;` với giá trị `true`.
        /// - `404 Not Found`: Không tìm thấy tài khoản với ID đã cung cấp.
        /// - `401 Unauthorized`: Không cung cấp token hợp lệ hoặc không có quyền truy cập.
        /// </returns>
        /// <response code="200">Xóa tài khoản thành công.</response>
        /// <response code="404">Trả về lỗi nếu không tìm thấy tài khoản với ID đã cung cấp.</response>
        /// <response code="401">Trả về lỗi nếu không cung cấp token hợp lệ hoặc không có quyền truy cập.</response>
        [HttpDelete(ApiEndPointConstant.Account.DeleteAccount)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteAccount([FromRoute] Guid id)
        {
            var response = await _accountService.DeleteAccount(id);
            return StatusCode(int.Parse(response.status), response);
        }
    }
}
