using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Enum;
using OhBau.Model.Exception;
using OhBau.Model.Paginate;
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
                throw new BadHttpRequestException("Không thể đặt lịch cho ngày trong quá khứ");
            }

            var existingBooking = await _unitOfWork.GetRepository<Booking>().SingleOrDefaultAsync(
                predicate: b => b.DotorSlotId.Equals(request.DotorSlotId) && b.Active == true && b.Date.Equals(request.Date));
            if (existingBooking != null)
            {
                throw new BadHttpRequestException("Đã có lịch đặt cho khung giờ này với bác sĩ này vào ngày này");
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

        public async Task<BaseResponse<IPaginate<GetBookingResponse>>> GetAllBookingForAdmin(int page, int size)
        {
            var bookings = await _unitOfWork.GetRepository<Booking>().GetPagingListAsync(
                selector: b => _mapper.Map<GetBookingResponse>(b),
                predicate: b => b.Active == true,
                include: b => b.Include(b => b.Parent)
                               .Include(b => b.DotorSlot)
                               .ThenInclude(ds => ds.Doctor)
                               .Include(b => b.DotorSlot.Slot),
                page: page,
                size: size
            );

            return new BaseResponse<IPaginate<GetBookingResponse>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Lấy danh sách booking thành công",
                data = bookings
            };
        }

        public async Task<BaseResponse<IPaginate<GetBookingResponse>>> GetAllBookingForDoctor(int page, int size)
        {
            Guid? userId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);

            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: a => a.Id.Equals(userId) && a.Active == true);
            if (account == null)
            {
                throw new NotFoundException("Không tìm thấy tài khoản");
            }

            var doctor = await _unitOfWork.GetRepository<Doctor>().SingleOrDefaultAsync(
                predicate: p => p.AccountId.Equals(userId) && p.Active == true);

            if (doctor == null)
            {
                throw new NotFoundException("Không tìm thấy thông tin bác sĩ");
            }

            var doctorSlot = await _unitOfWork.GetRepository<DoctorSlot>().GetListAsync(
                predicate: p => p.DoctorId.Equals(doctor.Id) && p.Active == true);

            var doctorSlotIds = doctorSlot.Select(ds => ds.Id).ToList();

            var bookings = await _unitOfWork.GetRepository<Booking>().GetPagingListAsync(
                selector: b => _mapper.Map<GetBookingResponse>(b),
                predicate: b => b.Active == true && doctorSlotIds.Contains(b.DotorSlotId),
                include: b => b.Include(b => b.Parent)
                               .Include(b => b.DotorSlot)
                               .ThenInclude(ds => ds.Doctor)
                               .Include(b => b.DotorSlot.Slot),
                page: page,
                size: size
            );

            return new BaseResponse<IPaginate<GetBookingResponse>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Lấy danh sách booking thành công",
                data = bookings
            };
        }

        public async Task<BaseResponse<IPaginate<GetBookingResponse>>> GetAllBookingForUser(int page, int size)
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

            var bookings = await _unitOfWork.GetRepository<Booking>().GetPagingListAsync(
                selector: b => _mapper.Map<GetBookingResponse>(b),
                predicate: b => b.Active == true && b.ParentId.Equals(parent.Id),
                include: b => b.Include(b => b.Parent)
                               .Include(b => b.DotorSlot)
                               .ThenInclude(ds => ds.Doctor)
                               .Include(b => b.DotorSlot.Slot),
                page: page,
                size: size
            );

            return new BaseResponse<IPaginate<GetBookingResponse>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Lấy danh sách booking thành công",
                data = bookings
            };
        }

        public async Task<BaseResponse<GetBookingResponse>> GetBookingById(Guid id)
        {
            var booking = await _unitOfWork.GetRepository<Booking>().SingleOrDefaultAsync(
                selector: b => _mapper.Map<GetBookingResponse>(b),
                predicate: b => b.Id.Equals(id) && b.Active == true);

            if (booking == null)
            {
                throw new NotFoundException("Không tìm thấy thông tin booking");
            }


            return new BaseResponse<GetBookingResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Lấy thông tin booking thành công",
                data = booking
            };
        }

        public async Task<BaseResponse<bool>> UpdateStatusBooking(Guid id, TypeBookingEnum type)
        {
            Guid? userId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);

            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: a => a.Id.Equals(userId) && a.Active == true);
            if (account == null)
            {
                throw new NotFoundException("Không tìm thấy tài khoản");
            }

            var doctor = await _unitOfWork.GetRepository<Doctor>().SingleOrDefaultAsync(
                predicate: p => p.AccountId.Equals(userId) && p.Active == true);

            if (doctor == null)
            {
                throw new NotFoundException("Không tìm thấy thông tin bác sĩ");
            }

            var booking = await _unitOfWork.GetRepository<Booking>().SingleOrDefaultAsync(
                predicate: b => b.Id.Equals(id) && b.DotorSlot.DoctorId.Equals(doctor.Id),
                include: b => b.Include(b => b.DotorSlot).ThenInclude(ds => ds.Doctor));

            if (booking == null)
            {
                throw new NotFoundException("Không tìm thấy booking hoặc booking không phải của bác sĩ này");
            }

            booking.Type = type.GetDescriptionFromEnum();
            booking.UpdateAt = TimeUtil.GetCurrentSEATime();

            _unitOfWork.GetRepository<Booking>().UpdateAsync(booking);
            await _unitOfWork.CommitAsync();

            return new BaseResponse<bool>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Cập nhật thành công",
                data = true
            };
        }
    }
}
