
using Microsoft.AspNetCore.Mvc;
using OhBau.API.Constants;
using OhBau.Model.Payload.Response;
using OhBau.Service.Interface;
using OhBau.Model.Payload.Response.ProductCategory;
using OhBau.Model.Payload.Request.ProductCategory;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Response.Product;

namespace OhBau.API.Controllers
{
    public class ProductCategoryController : BaseController<ProductCategoryController>
    {
        private readonly IProductCategoryService _productCategoryService;
        public ProductCategoryController(ILogger<ProductCategoryController> logger, IProductCategoryService productCategoryService) : base(logger)
        {
            _productCategoryService = productCategoryService;
        }

        [HttpPost(ApiEndPointConstant.ProductCategory.CreateProductCategory)]
        [ProducesResponseType(typeof(BaseResponse<CreateProductCategoryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CreateProductCategoryResponse>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> CreateProductCategory([FromBody] CreateProductCategoryRequest request)
        {
            var response = await _productCategoryService.CreaeteProductCategory(request);
            return StatusCode(int.Parse(response.status), response);
        }

        [HttpGet(ApiEndPointConstant.ProductCategory.GetAllProductCategory)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<GetProductCategoryResponse>>), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetAllProductCategory([FromQuery] int? page, [FromQuery] int? size)
        {
            int pageNumber = page ?? 1;
            int pageSize = size ?? 10;
            var response = await _productCategoryService.GetAllProductCategory(pageNumber, pageSize);
            return StatusCode(int.Parse(response.status), response);
        }

        [HttpGet(ApiEndPointConstant.ProductCategory.GetAllProductByCategory)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<GetProductResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<GetProductResponse>>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetAllProductByCategory([FromRoute] Guid id,[FromQuery] int? page, [FromQuery] int? size)
        {
            int pageNumber = page ?? 1;
            int pageSize = size ?? 10;
            var response = await _productCategoryService.GetAllProductByCategory(id, pageNumber, pageSize);
            return StatusCode(int.Parse(response.status), response);
        }

        [HttpGet(ApiEndPointConstant.ProductCategory.GetProductCategoryById)]
        [ProducesResponseType(typeof(BaseResponse<GetProductCategoryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetProductCategoryResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetProductCategoryById([FromRoute] Guid id)
        {
            var response = await _productCategoryService.GetProductCategoryById(id);
            return StatusCode(int.Parse(response.status), response);
        }
        
        [HttpDelete(ApiEndPointConstant.ProductCategory.DeleteProductCategory)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteProductCategory([FromRoute] Guid id)
        {
            var response = await _productCategoryService.DeleteProductCategory(id);
            return StatusCode(int.Parse(response.status), response);
        }

        [HttpPut(ApiEndPointConstant.ProductCategory.UpdateProductCategory)]
        [ProducesResponseType(typeof(BaseResponse<GetProductCategoryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetProductCategoryResponse>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateProductCategory([FromRoute] Guid id, [FromBody] UpdateProductCategoryRequest request)
        {
            var response = await _productCategoryService.UpdateProductCategory(id, request);
            return StatusCode(int.Parse(response.status), response);
        }
    }
}
