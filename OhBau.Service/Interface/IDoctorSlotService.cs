using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.DoctorSlot;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.DoctorSlot;

namespace OhBau.Service.Interface
{
    public interface IDoctorSlotService
    {
        Task<BaseResponse<List<CreateDoctorSlotResponse>>> CreateDoctorSlot(List<CreateDoctorSlotRequest> request);
        Task<BaseResponse<bool>> Active(Guid id);
        Task<BaseResponse<bool>> UnActive(Guid id);
        Task<BaseResponse<GetDoctorSlotsForUserResponse>> GetAllDoctorSlot(DateOnly date);
        Task<BaseResponse<GetDoctorSlotResponse>> GetDoctorSlot(Guid id, DateOnly date);
        Task<BaseResponse<GetDoctorSlotsForUserResponse>> GetAllDoctorSlotForUser(Guid id, DateOnly date);

    }
}
