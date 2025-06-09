using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Product;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Product;

namespace OhBau.Service.Interface
{
    public interface IProductService
    {
        Task<BaseResponse<CreateProductResponse>> CreateProduct(CreateProductRequest request);
        Task<BaseResponse<IPaginate<GetProductResponse>>> GetAllProduct(int page, int size);
        Task<BaseResponse<GetProductResponse>> GetProductById(Guid id);
        Task<BaseResponse<bool>> DeleteProduct(Guid id);
        Task<BaseResponse<GetProductResponse>> UpdateProduct(Guid id, UpdateProductRequest request);
    }
}
