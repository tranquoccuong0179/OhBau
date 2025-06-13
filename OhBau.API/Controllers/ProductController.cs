
using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Payload.Response;
using OhBau.Service.Interface;
using OhBau.Model.Payload.Response.Product;
using OhBau.Model.Payload.Request.Product;
using OhBau.Model.Paginate;
using OhBau.Service.Implement;

namespace OhBau.API.Controllers
{
    public class ProductController : BaseController<ProductController>
    {
        private readonly IProductService _productService;
        public ProductController(ILogger<ProductController> logger, IProductService productService) : base(logger)
        {
            _productService = productService;
        }

        [HttpPost(ApiEndPointConstant.Product.CreateProduct)]
        [ProducesResponseType(typeof(BaseResponse<CreateProductResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CreateProductResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequest request)
        {
            var response = await _productService.CreateProduct(request);
            return StatusCode(int.Parse(response.status), response);
        }

        [HttpGet(ApiEndPointConstant.Product.GetAllProduct)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<GetProductResponse>>), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetAllProduct([FromQuery] int? page, [FromQuery] int? size)
        {
            int pageNumber = page ?? 1;
            int pageSize = size ?? 10;
            var response = await _productService.GetAllProduct(pageNumber, pageSize);
            return StatusCode(int.Parse(response.status), response);
        }

        [HttpGet(ApiEndPointConstant.Product.GetProductById)]
        [ProducesResponseType(typeof(BaseResponse<GetProductResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetProductResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetProductById([FromRoute] Guid id)
        {
            var response = await _productService.GetProductById(id);
            return StatusCode(int.Parse(response.status), response);
        }
        
        [HttpPut(ApiEndPointConstant.Product.UpdateProduct)]
        [ProducesResponseType(typeof(BaseResponse<GetProductResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetProductResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductRequest request)
        {
            var response = await _productService.UpdateProduct(id, request);
            return StatusCode(int.Parse(response.status), response);
        }

        [HttpDelete(ApiEndPointConstant.Product.DeleteProduct)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
        {
            var response = await _productService.DeleteProduct(id);
            return StatusCode(int.Parse(response.status), response);
        }
    }
}
