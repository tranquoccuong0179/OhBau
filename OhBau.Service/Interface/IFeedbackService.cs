using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Payload.Request.Feedback;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Feedback;

namespace OhBau.Service.Interface
{
    public interface IFeedbackService
    {
        Task<BaseResponse<CreateFeedbackResponse>> CreateFeedback(CreateFeedbackRequest request);
    }
}
