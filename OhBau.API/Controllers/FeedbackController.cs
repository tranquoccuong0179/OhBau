
using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Exception;
using OhBau.Model.Payload.Response;
using OhBau.Service.Interface;
using OhBau.Model.Payload.Response.Feedback;
using OhBau.Model.Payload.Request.Feedback;

namespace OhBau.API.Controllers
{
    public class FeedbackController : BaseController<FeedbackController>
    {
        private readonly IFeedbackService _feedbackService;
        public FeedbackController(ILogger<FeedbackController> logger, IFeedbackService feedbackService) : base(logger)
        {
            _feedbackService = feedbackService;
        }

        [HttpPost(ApiEndPointConstant.Feedback.CreateFeedback)]
        [ProducesResponseType(typeof(BaseResponse<CreateFeedbackResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CreateFeedbackResponse>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackRequest request)
        {
            var response = await _feedbackService.CreateFeedback(request);
            return StatusCode(int.Parse(response.status), response);
        }
    }
}
