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
using OhBau.Model.Payload.Request.Feedback;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Feedback;
using OhBau.Model.Utils;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class FeedbackService : BaseService<FeedbackService>, IFeedbackService
    {
        public FeedbackService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<FeedbackService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<CreateFeedbackResponse>> CreateFeedback(CreateFeedbackRequest request)
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

            var booking = await _unitOfWork.GetRepository<Booking>().SingleOrDefaultAsync(
                predicate: b => b.Id.Equals(request.BookingId) && b.ParentId.Equals(parent.Id) && b.Type.Equals(TypeBookingEnum.Examined.GetDescriptionFromEnum()),
                include: b => b.Include(b => b.DotorSlot));

            if (booking == null)
            {
                throw new NotFoundException("Không tìm thấy thông tin booking");
            }

            var doctor = await _unitOfWork.GetRepository<Doctor>().SingleOrDefaultAsync(
                predicate : d => d.Id.Equals(request.DoctorId) && booking.DotorSlot.DoctorId.Equals(request.DoctorId));

            if (doctor == null)
            {
                throw new NotFoundException("Không tìm thấy thông tin doctor");
            }

            var existingFeedback = await _unitOfWork.GetRepository<Feedback>().SingleOrDefaultAsync(
                predicate: f => f.BookingId.Equals(request.BookingId));
            if (existingFeedback != null)
            {
                throw new BadHttpRequestException("Feedback cho đặt lịch này đã tồn tại.");
            }

            var feedback = _mapper.Map<Feedback>(request);

            await _unitOfWork.GetRepository<Feedback>().InsertAsync(feedback);
            await _unitOfWork.CommitAsync();

            return new BaseResponse<CreateFeedbackResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Tạo feedback thành công",
                data = _mapper.Map<CreateFeedbackResponse>(feedback)
            };
        }
    }
}
