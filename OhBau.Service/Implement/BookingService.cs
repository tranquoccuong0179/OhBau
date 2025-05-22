using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Exception;
using OhBau.Model.Payload.Request.Booking;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Booking;
using OhBau.Model.Utils;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class BookingService : BaseService<BookingService>, IBookingService
    {
        public BookingService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<BookingService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<CreateBookingResponse>> CreateBooking(CreateBookingRequest request)
        {
            Guid? userId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);

            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: a => a.Id.Equals(userId) && a.Active == true);
            if (account == null)
            {
                throw new NotFoundException("Không tìm thấy tài khoản");
            }

            var parent = await _unitOfWork.GetRepository<Parent>().SingleOrDefaultAsync(
                predicate: p => p.AccountId.Equals(userId) && p.Active == true);

            if (parent == null)
            {
                throw new NotFoundException("Không tìm thấy thông tin bố mẹ");
            }

            var doctorSlot = await _unitOfWork.GetRepository<DoctorSlot>().SingleOrDefaultAsync(
                predicate: d => d.Id.Equals(request.DotorSlotId) && d.Active == true);

            if (doctorSlot == null)
            {
                throw new NotFoundException("Không tìm thấy khung giờ của bác sĩ");
            }

            if (request.Date < DateOnly.FromDateTime(DateTime.Today))
            {
                throw new InvalidOperationException("Không thể đặt lịch cho ngày trong quá khứ");
            }

            var existingBooking = await _unitOfWork.GetRepository<Booking>().SingleOrDefaultAsync(
                predicate: b => b.DotorSlotId.Equals(request.DotorSlotId) && b.Active == true && b.Date.Equals(request.Date));
            if (existingBooking != null)
            {
                throw new InvalidOperationException("Đã có lịch đặt cho khung giờ này với bác sĩ này vào ngày này");
            }

            var booking = _mapper.Map<Booking>(request);
            booking.ParentId = parent.Id;

            await _unitOfWork.GetRepository<Booking>().InsertAsync(booking);
            await _unitOfWork.CommitAsync();


            return new BaseResponse<CreateBookingResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Đăng kí lịch thành công",
                data = _mapper.Map<CreateBookingResponse>(booking)
            };
        }
    }
}
