
using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.ParentRelation;
using OhBau.Service.Implement;
using OhBau.Service.Interface;

namespace OhBau.API.Controllers
{
    public class ParentRelationController : BaseController<ParentRelationController>
    {
        private readonly IParentRelationService _parentRelationService;
        public ParentRelationController(ILogger<ParentRelationController> logger, IParentRelationService parentRelationService) : base(logger)
        {
            _parentRelationService = parentRelationService;
        }

        [HttpGet(ApiEndPointConstant.ParentRelation.GetParentRelation)]
        [ProducesResponseType(typeof(BaseResponse<GetParentRelationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetParentRelationResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetParentRelation()
        {
            var response = await _parentRelationService.GetParentRelation();
            return StatusCode(int.Parse(response.status), response);
        }
    }
}
