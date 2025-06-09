using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Payload.Request.Product;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Product;

namespace OhBau.Service.Interface
{
    public interface IProductService
    {
        Task<BaseResponse<CreateProductResponse>> CreateProduct(CreateProductRequest request);
    }
}
