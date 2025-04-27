
using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Account;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Account;
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
        /// - Trường `role` trong yêu cầu phải là một trong các giá trị sau: `FATHER`, `MOTHER`, hoặc `DOCTOR`.
        /// - Tất cả các trường trong `RegisterRequest` đều bắt buộc.
        /// - Không yêu cầu xác thực (public API).
        /// - Ví dụ nội dung yêu cầu:
        /// ```json
        /// {
        ///   "phone": "0987654321",
        ///   "email": "cuongtq@gmail.com",
        ///   "password": "123456",
        ///   "role": "FATHER/MOTHER/DOCTOR"
        /// }
        /// ```
        /// </remarks>
        /// <param name="request">Thông tin đăng ký của người dùng. Phải bao gồm `phone`, `email`, `password`, và `role` (giá trị cho phép: `FATHER`, `MOTHER`, `DOCTOR`).</param>
        /// <returns>
        /// - `200 OK`: Đăng ký thành công.  
        /// - `400 Bad Request`: Thông tin đầu vào không hợp lệ (ví dụ: thiếu trường, email không hợp lệ, hoặc `role` không được hỗ trợ).
        /// </returns>
        /// <response code="200">Trả về kết quả đăng ký khi tài khoản được tạo thành công.</response>
        /// <response code="400">Trả về lỗi nếu yêu cầu không hợp lệ hoặc `role` không phải là `FATHER`, `MOTHER`, hoặc `DOCTOR`.</response>
        [HttpPost(ApiEndPointConstant.Account.RegisterAccount)]
        [ProducesResponseType(typeof(BaseResponse<RegisterResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<RegisterResponse>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> CreateNewAccount([FromBody] RegisterRequest request)
        {
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
        /// - Ví dụ yêu cầu:
        ///   ```
        ///   GET /api/v1/account?page=1&amp;size=10
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
    }
}
