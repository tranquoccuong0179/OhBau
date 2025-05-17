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
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Blog;
using OhBau.Model.Payload.Request.Slot;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Slot;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class SlotService : BaseService<SlotService>, ISlotService
    {
        public SlotService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<SlotService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<CreateSlotResponse>> CreateSlot(CreateSlotRequest request)
        {

            var isNameExists = await _unitOfWork.GetRepository<Slot>()
                .SingleOrDefaultAsync(predicate: s => s.Name == request.Name);

            if (isNameExists != null)
            {
                throw new BadHttpRequestException("Tên slot đã tồn tại");
            }

            var isTimeConflict = await _unitOfWork.GetRepository<Slot>().SingleOrDefaultAsync(
                predicate: s =>
                    (request.StartTime >= s.StartTime && request.StartTime < s.EndTime) ||
                    (request.EndTime > s.StartTime && request.EndTime <= s.EndTime) ||
                    (request.StartTime <= s.StartTime && request.EndTime >= s.EndTime)
            );

            if (isTimeConflict != null)
            {
                throw new BadHttpRequestException("Khoảng thời gian bị trùng với slot khác");
            }
            var slot = _mapper.Map<Slot>(request);

            await _unitOfWork.GetRepository<Slot>().InsertAsync(slot);
            await _unitOfWork.CommitAsync();

            return new BaseResponse<CreateSlotResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Tạo thành công slot",
                data = _mapper.Map<CreateSlotResponse>(slot)
            };
        }

        public async Task<BaseResponse<IPaginate<GetSlotResponse>>> GetAllSlot(int page, int size)
        {
            var slots = await _unitOfWork.GetRepository<Slot>().GetPagingListAsync(
                selector: s => _mapper.Map<GetSlotResponse>(s),
                predicate: s => s.Active == true,
                page: page,
                size: size);

            return new BaseResponse<IPaginate<GetSlotResponse>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Lấy danh sách slot thành công",
                data = slots
            };
        }

        public async Task<BaseResponse<GetSlotResponse>> GetSlot(Guid id)
        {
            var slot = await _unitOfWork.GetRepository<Slot>().SingleOrDefaultAsync(
                selector: s => _mapper.Map<GetSlotResponse>(s),
                predicate: s => s.Id.Equals(id) && s.Active == true);

            if (slot == null)
            {
                throw new NotFoundException("Không tìm thấy slot");
            }

            return new BaseResponse<GetSlotResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Lấy thông tin slot thành công",
                data = slot
            };
        }
    }
}
