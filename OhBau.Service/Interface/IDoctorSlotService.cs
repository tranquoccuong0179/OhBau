using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Payload.Request.DoctorSlot;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.DoctorSlot;

namespace OhBau.Service.Interface
{
    public interface IDoctorSlotService
    {
        Task<BaseResponse<CreateDoctorSlotReponse>> CreateDoctorSlot(CreateDoctorSlotRequest request);
    }
}
