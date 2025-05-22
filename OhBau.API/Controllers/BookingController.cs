
using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Payload.Response;
using OhBau.Service.Interface;
using OhBau.Model.Payload.Response.Booking;
using OhBau.Model.Payload.Request.Booking;

namespace OhBau.API.Controllers
{
    public class BookingController : BaseController<BookingController>
    {
        private readonly IBookingService _bookingService;
        public BookingController(ILogger<BookingController> logger, IBookingService bookingService) : base(logger)
        {
            _bookingService = bookingService;
        }

        [HttpPost(ApiEndPointConstant.Booking.CreateBooking)]
        [ProducesResponseType(typeof(BaseResponse<CreateBookingResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CreateBookingResponse>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
        {
            var response = await _bookingService.CreateBooking(request);
            return StatusCode(int.Parse(response.status), response);
        }
    }
}
