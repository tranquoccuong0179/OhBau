using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Payload.Request.Booking;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Booking;

namespace OhBau.Service.Interface
{
    public interface IBookingService
    {
        Task<BaseResponse<CreateBookingResponse>> CreateBooking(CreateBookingRequest request);
    }
}
