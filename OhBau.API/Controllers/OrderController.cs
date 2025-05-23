using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OhBau.Model.Payload.Request.Order;
using OhBau.Model.Utils;
using OhBau.Service.Interface;

namespace OhBau.API.Controllers
{
    [ApiController]
    [Route("api/v1/order")]
    public class OrderController(IOrderSerivce _orderService, ILogger<OrderController> _logger) : Controller
    {
        [HttpPost("create-order")]
        [Authorize(Roles = "FATHER,MOTHER")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var response = await _orderService.CreateOrder(request);
                return StatusCode(int.Parse(response.status), response);
            }
            catch (Exception ex) {

                _logger.LogError("[Create Order API] " + ex.Message,ex.StackTrace);
                return StatusCode(500,ex.ToString());
            }
        }

        [HttpGet("get-orders")]
        [Authorize]
        public async Task<IActionResult> GetOrders([FromQuery]int pageNumber, [FromQuery]int pageSize)
        {
            var accountId = UserUtil.GetAccountId(HttpContext);
            var response = await _orderService.GetOrders(accountId!.Value, pageNumber, pageSize);
            return StatusCode(int.Parse(response.status),response);
        }

        [HttpGet("get-order-details")]
        [Authorize]
        public async Task<IActionResult> GetOrderDetails([FromQuery] Guid orderId, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var accountId = UserUtil.GetAccountId(HttpContext);
            var response = await _orderService.GetOrderDetails(accountId!.Value,orderId, pageNumber, pageSize);
            return StatusCode(int.Parse(response.status), response);
        }
    }
}
