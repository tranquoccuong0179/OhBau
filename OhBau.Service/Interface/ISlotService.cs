using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Payload.Request.Blog;
using OhBau.Model.Payload.Request.Slot;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Slot;

namespace OhBau.Service.Interface
{
    public interface ISlotService
    {
        Task<BaseResponse<CreateSlotResponse>> CreateSlot(CreateSlotRequest request);
    }
}
