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
using OhBau.Model.Payload.Request.MotherHealth;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.MotherHealth;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class MotherHealthService : BaseService<MotherHealthService>, IMotherHealthService
    {
        public MotherHealthService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<MotherHealthService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<GetMotherHealthResponse>> GetMotherHealth(Guid id)
        {
            var motherHealth = await _unitOfWork.GetRepository<MotherHealthRecord>().SingleOrDefaultAsync(
                predicate: m => m.Id.Equals(id) && m.Active == true);

            if (motherHealth == null)
            {
                throw new NotFoundException("Không tìm thấy ghi chú sức khỏe của mẹ");
            }
            var response = _mapper.Map<GetMotherHealthResponse>(motherHealth);

            return new BaseResponse<GetMotherHealthResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Lấy thông tin sức khỏe của mẹ thành công",
                data = response
            };
        }

        public async Task<BaseResponse<UpdateMotherHealthResponse>> UpdateMotherHealth(Guid id, UpdateMotherHealthRequest request)
        {
            var motherHealth = await _unitOfWork.GetRepository<MotherHealthRecord>().SingleOrDefaultAsync(
                predicate: m => m.Id.Equals(id) && m.Active == true);

            if (motherHealth == null)
            {
                throw new NotFoundException("Không tìm thấy ghi chú sức khỏe của mẹ");
            }

            motherHealth.Weight = request.Weight.HasValue ? request.Weight.Value : motherHealth.Weight;
            motherHealth.BloodPressure = request.BloodPressure.HasValue ? request.BloodPressure.Value : motherHealth.BloodPressure;

            _unitOfWork.GetRepository<MotherHealthRecord>().UpdateAsync(motherHealth);
            await _unitOfWork.CommitAsync();

            var response = _mapper.Map<UpdateMotherHealthResponse>(motherHealth);

            return new BaseResponse<UpdateMotherHealthResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Cập nhật ghi chú sức khỏe của mẹ thành công",
                data = response
            };
        }
    }
}
