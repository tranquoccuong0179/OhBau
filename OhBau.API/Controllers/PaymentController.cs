using Microsoft.AspNetCore.Mvc;
using VNPayService.DTO;
using VNPayService;

namespace OhBau.API.Controllers
{
    [ApiController]
    [Route("api/v1/payment")]
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly ILogger<PaymentController> _logger;
        public PaymentController(IVnPayService vnPayService, ILogger<PaymentController> logger)
        {
            _logger = logger;
            _vnPayService = vnPayService;
        }

        [HttpGet("create-payment")]
        public async Task<IActionResult> CreatePayment([FromQuery] CreateOrder request)
        {
            try
            {
                var result = await _vnPayService.CreatePayment(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("VN Pay Create Payment API]", ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("return-payment")]
        public async Task<IActionResult> ReturnPayment()
        {
            try
            {
                var result = await _vnPayService.VnPayReturn();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Return VN Pay Payment]", ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());

            }
        }

        [HttpGet("process-payment")]
        public async Task<IActionResult> ProcessPayment()
        {
            try
            {
                var queryParams = HttpContext.Request.Query
                           .ToDictionary(k => k.Key, v => v.Value.ToString());

                var result = await _vnPayService.ProcessVnPayReturn(queryParams);

                if (result.IsSuccessful)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.StackTrace);
                return StatusCode(500,ex.ToString());
            }
        }
    }
}
