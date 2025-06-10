using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.ProductCategory;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Product;
using OhBau.Model.Payload.Response.ProductCategory;

namespace OhBau.Service.Interface
{
    public interface IProductCategoryService
    {
        Task<BaseResponse<CreateProductCategoryResponse>> CreaeteProductCategory(CreateProductCategoryRequest request);
        Task<BaseResponse<IPaginate<GetProductCategoryResponse>>> GetAllProductCategory(int page, int size);
        Task<BaseResponse<GetProductCategoryResponse>> GetProductCategoryById(Guid id);
        Task<BaseResponse<GetProductCategoryResponse>> UpdateProductCategory(Guid id, UpdateProductCategoryRequest request);
        Task<BaseResponse<IPaginate<GetProductResponse>>> GetAllProductByCategory(Guid id, int page, int size);
        Task<BaseResponse<bool>> DeleteProductCategory(Guid id);
    }
}
