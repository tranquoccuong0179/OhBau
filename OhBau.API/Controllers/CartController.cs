﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OhBau.Model.Payload.Request.Order;
using OhBau.Model.Utils;
using OhBau.Service.Interface;

namespace OhBau.API.Controllers
{
    [ApiController]
    [Route("api/v1/cart")]
    public class CartController(ICartService _cartService, ILogger<CartController> _logger) : Controller
    {
        [HttpPost("add-course-to-cart")]
        [Authorize(Roles = "FATHER,MOTHER")]
        public async Task<IActionResult> AddCourseToCart([FromBody] AddCourseToOrderRequest request)
        {
            try
            {
                var accountId = UserUtil.GetAccountId(HttpContext);
                var response = await _cartService.AddCourseToCart(request.CourseId, accountId!.Value);
                return StatusCode(int.Parse(response.status), response);   
            }
            catch (Exception ex) {
                _logger.LogError("[Add Course To Order]" + ex.Message,ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }

        }

        [HttpGet("get-cart-items-by-account")]
        [Authorize(Roles = "FATHER,MOTHER")]
        public async Task<IActionResult> GetCartByAccount([FromQuery] int pageNumber,[FromQuery]int pageSize)
        {
            
                var accountId = UserUtil.GetAccountId(HttpContext);
                var response = await _cartService.GetCartItemByAccount(accountId!.Value, pageNumber, pageSize);
                return StatusCode(int.Parse(response.status),response);
        }

        [HttpGet("get-cart-items-details")]
        [Authorize(Roles = "FATHER,MOTHER")]
        public async Task<IActionResult> GetCartDetails([FromQuery]int pageNumber, [FromQuery] int pageSize)
        {
            var accountId = UserUtil.GetAccountId(HttpContext);
            var response = await _cartService.GetCartDetails(accountId!.Value, pageNumber, pageSize);
            return StatusCode(int.Parse(response.status), response);
        }

        [HttpDelete("delete-cart-item")]
        public async Task<IActionResult> DeleteCartItem([FromQuery] Guid itemId)
        {
            try
            {
                var response = await _cartService.DeleteCartItem(itemId);
                return StatusCode(int.Parse(response.status), response);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Delete cart item API]:" + ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }
    }
}
