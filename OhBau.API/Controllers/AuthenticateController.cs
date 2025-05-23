
using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Payload.Response;
using OhBau.Service.Interface;
using OhBau.Model.Payload.Response.Authentication;
using OhBau.Model.Payload.Request.Authentication;
using EmailService.Service;

namespace OhBau.API.Controllers
{
    public class AuthenticateController : BaseController<AuthenticateController>
    {
        private readonly IAuthService _authService;
        private readonly IEmailSender _emailSender;
        public AuthenticateController(ILogger<AuthenticateController> logger, IAuthService authService, IEmailSender emailSender) : base(logger)
        {
            _authService = authService;
            _emailSender = emailSender;
        }

        /// <summary>
        /// API xác thực người dùng và trả về token.
        /// </summary>
        /// <remarks>
        /// - API này cho phép người dùng xác thực bằng thông tin đăng nhập thông qua `AuthenticationRequest`.
        /// - Không yêu cầu xác thực (public API).
        /// - Ví dụ yêu cầu:
        ///   ```
        ///   POST /api/v1/account
        ///   ```
        /// - Ví dụ nội dung yêu cầu:
        ///   ```json
        ///   {
        ///     "phone": "0987654321",
        ///     "password": "123456",
        ///    }
        /// - Kết quả trả về:
        ///   - `200 OK`: Xác thực thành công. Trả về `BaseResponse&lt;AuthenticationResponse&gt;` chứa token và thông tin người dùng.
        ///   - `400 Bad Request`: Thông tin đầu vào không hợp lệ (ví dụ: thiếu trường, phone hoặc mật khẩu không đúng).
        /// - Ví dụ phản hồi thành công (200 OK):
        ///   ```json
        ///   {
        ///     "status": "200",
        ///     "data": {
        ///       "Id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///       "Phone": "0987654321",
        ///       "Role": "FATHER",
        ///       "AccessToken": "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAx..."
        ///     },
        ///     "message": "Đăng nhập thành công"
        ///   }
        ///   ```
        /// </remarks>
        /// <param name="request">Thông tin xác thực của người dùng. Phải bao gồm `phone` và `password`.</param>
        /// <returns>
        /// - `200 OK`: Xác thực thành công. Trả về `BaseResponse&lt;AuthenticationResponse&gt;` chứa token và thông tin người dùng.
        /// - `400 Bad Request`: Thông tin đầu vào không hợp lệ (ví dụ: thiếu trường, phone hoặc mật khẩu không đúng).
        /// </returns>
        /// <response code="200">Trả về token và thông tin người dùng khi xác thực thành công.</response>
        /// <response code="400">Trả về lỗi nếu thông tin xác thực không hợp lệ.</response>
        [HttpPost(ApiEndPointConstant.Authentication.Auth)]
        [ProducesResponseType(typeof(BaseResponse<AuthenticationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<AuthenticationResponse>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticationRequest request)
        {
            var response = await _authService.Authenticate(request);
            return StatusCode(int.Parse(response.status), response);
        }


        [HttpPost("send")]
        public async Task<IActionResult> SendEmail(string email, string subject, string message)
        {
            try
            {
                 await _emailSender.SendEmailAsync(email, subject, message);
                return Ok("Email sent successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error sending email: {ex.Message}");
            }
        }
    }
}
