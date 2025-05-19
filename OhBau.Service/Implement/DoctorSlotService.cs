using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Exception;
using OhBau.Model.Paginate;
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

        public async Task<BaseResponse<bool>> Active(Guid id)
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

            var doctorSlot = await _unitOfWork.GetRepository<DoctorSlot>().SingleOrDefaultAsync(
                predicate: ds => ds.Id.Equals(id) && ds.DoctorId.Equals(doctor.Id));
            if (doctorSlot == null)
            {
                throw new NotFoundException("Khung giờ khám không tồn tại hoặc không thuộc về bác sĩ này");
            }

            doctorSlot.Active = true;

            _unitOfWork.GetRepository<DoctorSlot>().UpdateAsync(doctorSlot);
            await _unitOfWork.CommitAsync();

            return new BaseResponse<bool>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Kích hoạt slot cho bác sĩ thành công",
                data = true
            };
        }

        public async Task<BaseResponse<List<CreateDoctorSlotResponse>>> CreateDoctorSlot(List<CreateDoctorSlotRequest> requests)
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

            if (requests == null || !requests.Any())
            {
                throw new BadHttpRequestException("Danh sách yêu cầu không được rỗng");
            }

            var createdDoctorSlots = new List<CreateDoctorSlotResponse>();
            var doctorSlotsToInsert = new List<DoctorSlot>();
            var slotIds = requests.Select(r => r.SlotId).Distinct().ToList();

            var slots = await _unitOfWork.GetRepository<Slot>()
                .GetListAsync(predicate: s => slotIds.Contains(s.Id) && s.Active == true);

            if (slots.Count != slotIds.Count)
            {
                var missingSlotIds = slotIds.Except(slots.Select(s => s.Id)).ToList();
                throw new NotFoundException($"Không tìm thấy khung giờ với ID: {string.Join(", ", missingSlotIds)}");
            }

            var existingDoctorSlots = await _unitOfWork.GetRepository<DoctorSlot>()
                .GetListAsync(predicate: ds => ds.DoctorId.Equals(doctor.Id) && slotIds.Contains(ds.SlotId) && ds.Active == true);

            var existingSlotIds = existingDoctorSlots.Select(ds => ds.SlotId).ToList();
            var duplicateSlotIds = slotIds.Intersect(existingSlotIds).ToList();

            if (duplicateSlotIds.Any())
            {
                throw new BadHttpRequestException($"Bác sĩ đã thêm khung giờ với ID: {string.Join(", ", duplicateSlotIds)}");
            }

            foreach (var request in requests)
            {
                var slot = slots.First(s => s.Id == request.SlotId);
                var doctorSlot = _mapper.Map<DoctorSlot>(request);
                doctorSlot.DoctorId = doctor.Id;
                doctorSlot.SlotId = slot.Id;
                doctorSlot.Active = true;

                doctorSlotsToInsert.Add(doctorSlot);
                createdDoctorSlots.Add(_mapper.Map<CreateDoctorSlotResponse>(doctorSlot));
            }

            await _unitOfWork.GetRepository<DoctorSlot>().InsertRangeAsync(doctorSlotsToInsert);
            await _unitOfWork.CommitAsync();

            return new BaseResponse<List<CreateDoctorSlotResponse>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Bác sĩ thêm các khung giờ thành công",
                data = createdDoctorSlots
            };
        }

        public async Task<BaseResponse<IPaginate<GetDoctorSlotResponse>>> GetAllDoctorSlot(int page, int size)
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

            var doctorSlot = await _unitOfWork.GetRepository<DoctorSlot>().GetPagingListAsync(
                selector: ds => _mapper.Map<GetDoctorSlotResponse>(ds),
                predicate: ds => ds.DoctorId.Equals(doctor.Id),
                page: page,
                size: size);

            return new BaseResponse<IPaginate<GetDoctorSlotResponse>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Lấy danh sách khung giờ của bác sĩ thành công",
                data = doctorSlot
            };
        }

        public async Task<BaseResponse<GetDoctorSlotResponse>> GetDoctorSlot(Guid id)
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

            var doctorSlot = await _unitOfWork.GetRepository<DoctorSlot>().SingleOrDefaultAsync(
                selector: ds => _mapper.Map<GetDoctorSlotResponse>(ds),
                predicate: ds => ds.Id.Equals(id) && ds.DoctorId.Equals(doctor.Id));
            if (doctorSlot == null)
            {
                throw new NotFoundException("Khung giờ khám không tồn tại hoặc không thuộc về bác sĩ này");
            }

            return new BaseResponse<GetDoctorSlotResponse>()
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Lấy thông tin khung giờ khám của bác sĩ thành công",
                data = doctorSlot
            };
        }

        public async Task<BaseResponse<bool>> UnActive(Guid id)
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

            var doctorSlot = await _unitOfWork.GetRepository<DoctorSlot>().SingleOrDefaultAsync(
                predicate: ds => ds.Id.Equals(id) && ds.DoctorId.Equals(doctor.Id));
            if (doctorSlot == null)
            {
                throw new NotFoundException("Khung giờ khám không tồn tại hoặc không thuộc về bác sĩ này");
            }

            doctorSlot.Active = false;

            _unitOfWork.GetRepository<DoctorSlot>().UpdateAsync(doctorSlot);
            await _unitOfWork.CommitAsync();

            return new BaseResponse<bool>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Hủy slot cho bác sĩ thành công",
                data = true
            };
        }
    }
}
