using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Booking;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Booking;

namespace OhBau.Service.Interface
{
    public interface IBookingService
    {
        Task<BaseResponse<CreateBookingResponse>> CreateBooking(CreateBookingRequest request);
        Task<BaseResponse<IPaginate<GetBookingResponse>>> GetAllBookingForAdmin(int page, int size);
        Task<BaseResponse<IPaginate<GetBookingResponse>>> GetAllBookingForUser(int page, int size);
        Task<BaseResponse<IPaginate<GetBookingResponse>>> GetAllBookingForDoctor(int page, int size);
        Task<BaseResponse<GetBookingResponse>> GetBookingById(Guid id);
    }
}
