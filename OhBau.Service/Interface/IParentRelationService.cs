using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.ParentRelation;

namespace OhBau.Service.Interface
{
    public interface IParentRelationService
    {
        Task<BaseResponse<GetParentRelationResponse>> GetParentRelation();
    }
}
