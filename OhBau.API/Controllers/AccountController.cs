
using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Payload.Request;
using OhBau.Model.Payload.Response;
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
    }
}
