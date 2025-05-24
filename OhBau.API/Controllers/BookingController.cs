
using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Payload.Response;
using OhBau.Service.Interface;
using OhBau.Model.Payload.Response.Booking;
using OhBau.Model.Payload.Request.Booking;
using OhBau.Model.Paginate;

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

        [HttpGet(ApiEndPointConstant.Booking.GetAllBookingForAdmin)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<GetBookingResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<GetBookingResponse>>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetAllBookingForAdmin([FromQuery] int? page, [FromQuery] int? size)
        {
            int pageNumber = page ?? 1;
            int pageSize = size ?? 10;
            var response = await _bookingService.GetAllBookingForAdmin(pageNumber, pageSize);
            return StatusCode(int.Parse(response.status), response);
        }
        
        [HttpGet(ApiEndPointConstant.Booking.GetAllBookingForDoctor)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<GetBookingResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<GetBookingResponse>>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetAllBookingForDoctor([FromQuery] int? page, [FromQuery] int? size)
        {
            int pageNumber = page ?? 1;
            int pageSize = size ?? 10;
            var response = await _bookingService.GetAllBookingForDoctor(pageNumber, pageSize);
            return StatusCode(int.Parse(response.status), response);
        }
        
        [HttpGet(ApiEndPointConstant.Booking.GetAllBookingForUser)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<GetBookingResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<GetBookingResponse>>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetAllBookingForUser([FromQuery] int? page, [FromQuery] int? size)
        {
            int pageNumber = page ?? 1;
            int pageSize = size ?? 10;
            var response = await _bookingService.GetAllBookingForUser(pageNumber, pageSize);
            return StatusCode(int.Parse(response.status), response);
        }
    }
}
