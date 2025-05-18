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
using OhBau.Model.Payload.Request.DoctorSlot;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.DoctorSlot;
using OhBau.Model.Utils;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class DoctorSlotService : BaseService<DoctorSlotService>, IDoctorSlotService
    {
        public DoctorSlotService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<DoctorSlotService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<CreateDoctorSlotReponse>> CreateDoctorSlot(CreateDoctorSlotRequest request)
        {
            Guid? userId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);

            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: a => a.Id.Equals(userId) && a.Active == true);
            
            if (account == null)
            {
                throw new NotFoundException("Không tìm thấy tài khoản");
            }

            var doctor = await _unitOfWork.GetRepository<Doctor>().SingleOrDefaultAsync(
                predicate: d => d.AccountId.Equals(userId) && d.Active == true);

            if (doctor == null)
            {
                throw new NotFoundException("Không tìm thấy thông tin bác sĩ");
            }

            var slot = await _unitOfWork.GetRepository<Slot>().SingleOrDefaultAsync(
                predicate: s => s.Id.Equals(request.SlotId) && s.Active == true);

            if (slot == null)
            {
                throw new NotFoundException("Không tìm thấy khung giờ đã chọn");
            }

            var existingDoctorSlot = await _unitOfWork.GetRepository<DoctorSlot>().SingleOrDefaultAsync(
                predicate: ds => ds.SlotId.Equals(request.SlotId) && ds.DoctorId.Equals(doctor.Id) && ds.Active == true);

            if (existingDoctorSlot != null)
            {
                throw new BadHttpRequestException("Bác sĩ đã thêm khung giờ này rồi");
            }

            var doctorSlot = _mapper.Map<DoctorSlot>(slot);
            doctorSlot.DoctorId = doctor.Id;

            await _unitOfWork.GetRepository<DoctorSlot>().InsertAsync(doctorSlot);
            await _unitOfWork.CommitAsync();

            return new BaseResponse<CreateDoctorSlotReponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Bác sĩ thêm khung giờ thành công",
                data = _mapper.Map<CreateDoctorSlotReponse>(doctorSlot)
            };
        }
    }
}
