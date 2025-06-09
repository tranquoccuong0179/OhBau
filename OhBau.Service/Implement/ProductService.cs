using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Enum;
using OhBau.Model.Exception;
using OhBau.Model.Payload.Request.Product;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Product;
using OhBau.Model.Utils;
using OhBau.Repository.Interface;
using OhBau.Service.CloudinaryService;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class ProductService : BaseService<ProductService>, IProductService
    {
        private readonly ICloudinaryService _cloudinaryService;
        public ProductService(IUnitOfWork<OhBauContext> unitOfWork, 
                              ILogger<ProductService> logger, 
                              IMapper mapper, 
                              IHttpContextAccessor httpContextAccessor,
                              ICloudinaryService cloudinaryService) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _cloudinaryService = cloudinaryService;
        }

        public async Task<BaseResponse<CreateProductResponse>> CreateProduct(CreateProductRequest request)
        {

            var productCategory = await _unitOfWork.GetRepository<ProductCategory>().SingleOrDefaultAsync(
                predicate: pc => pc.Id.Equals(request.CategoryId));

            if (productCategory == null)
            {
                throw new NotFoundException("Không tìm thấy danh mục sản phẩm");
            }

            var uploadResponse = await _cloudinaryService.Upload(request.Image);

            if (uploadResponse.data == null)
            {
                throw new Exception($"Upload failed: {uploadResponse.message}");
            }

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Brand = request.Brand,
                Price = request.Price,
                Quantity = request.Quantity,
                Color = request.Color,
                Size = request.Size,
                AgeRange = request.AgeRange,
                Image = uploadResponse.data,
                //Status = request.Status.GetDescriptionFromEnum(),
                Status = ProductStatusEnum.InStock.GetDescriptionFromEnum(),
                CategoryId = request.CategoryId,
                Active = true,
                CreatedAt = TimeUtil.GetCurrentSEATime(),
                UpdatedAt = TimeUtil.GetCurrentSEATime(),
            };

            await _unitOfWork.GetRepository<Product>().InsertAsync(product);

            await _unitOfWork.CommitAsync();

            return new BaseResponse<CreateProductResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Tạo sản phẩm thành công",
                data = new CreateProductResponse
                {
                    Name = product.Name,
                    Description = product.Description,
                    Brand = product.Brand,
                    Price = product.Price,
                    Quantity = product.Quantity,
                    Color = product.Color,
                    Size = product.Size,
                    AgeRange = product.AgeRange,
                    Image = product.Image,
                    Status = product.Status,
                    CategoryId = product.CategoryId
                }
            };
        }
    }
}
