
using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Exception;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Request.Fetus;
using OhBau.Model.Payload.Response.Fetus;
using OhBau.Service.Interface;
using OhBau.Model.Paginate;

namespace OhBau.API.Controllers
{
    public class FetusController : BaseController<FetusController>
    {
        private readonly IFetusService _fetusService;
        public FetusController(ILogger<FetusController> logger, IFetusService fetusService) : base(logger)
        {
            _fetusService = fetusService;
        }

        [HttpPost(ApiEndPointConstant.Fetus.CreateFetus)]
        [ProducesResponseType(typeof(BaseResponse<CreateFetusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CreateFetusResponse>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> CreateFetus([FromBody] CreateFetusRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );
                throw new ModelValidationException(errors);
            }
            var response = await _fetusService.CreateFetus(request);
            return StatusCode(int.Parse(response.status), response);
        }


        [HttpGet(ApiEndPointConstant.Fetus.GetAllFetus)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<GetFetusResponse>>), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetAllFetus([FromQuery] int? page, [FromQuery] int? size)
        {
            int pageNumber = page ?? 1;
            int pageSize = size ?? 10;
            var response = await _fetusService.GetAllFetus(pageNumber, pageSize);
            return StatusCode(int.Parse(response.status), response);
        }


        [HttpGet(ApiEndPointConstant.Fetus.GetFetusById)]
        [ProducesResponseType(typeof(BaseResponse<GetFetusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetFetusResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _fetusService.GetFetusById(id);
            return StatusCode(int.Parse(response.status), response);
        }


        [HttpGet(ApiEndPointConstant.Fetus.GetFetusByCode)]
        [ProducesResponseType(typeof(BaseResponse<GetFetusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetFetusResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetByCode([FromRoute] string code)
        {
            var response = await _fetusService.GetFetusByCode(code);
            return StatusCode(int.Parse(response.status), response);
        }
    }
}
