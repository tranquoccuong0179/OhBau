
using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Payload.Request.Parent;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Parent;
using OhBau.Service.Interface;

namespace OhBau.API.Controllers
{
    public class ParentController : BaseController<ParentController>
    {
        private readonly IParentService _parentService;
        public ParentController(ILogger<ParentController> logger, IParentService parentService) : base(logger)
        {
            _parentService = parentService;
        }
        [HttpGet(ApiEndPointConstant.Parent.CreateParent)]
        [ProducesResponseType(typeof(BaseResponse<RegisterParentResponse>), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> CreateParent([FromBody] RegisterParentRequest request) 
        {
            var response = await _parentService.AddNewParent(request);
            return StatusCode(int.Parse(response.status), response);
        }
    }
}
