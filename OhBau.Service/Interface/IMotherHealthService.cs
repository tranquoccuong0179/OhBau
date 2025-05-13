using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Payload.Request.MotherHealth;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.MotherHealth;

namespace OhBau.Service.Interface
{
    public interface IMotherHealthService
    {
        Task<BaseResponse<UpdateMotherHealthResponse>> UpdateMotherHealth(Guid id, UpdateMotherHealthRequest request);
        Task<BaseResponse<GetMotherHealthResponse>> GetMotherHealth(Guid id);
    }
}
