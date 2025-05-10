using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Payload.Request.Parent;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Parent;

namespace OhBau.Service.Interface
{
    public interface IParentService
    {
        Task<BaseResponse<RegisterParentResponse>> AddNewParent(RegisterParentRequest request);

    }
}
