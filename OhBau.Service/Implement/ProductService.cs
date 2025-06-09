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
using OhBau.Model.Paginate;
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

        public async Task<BaseResponse<bool>> DeleteProduct(Guid id)
        {
            var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
                predicate: p => p.Id.Equals(id) && p.Active);

            if (product == null)
            {
                throw new NotFoundException("Không tìm thấy sản phẩm");
            }

            product.Active = false;
            product.UpdatedAt = TimeUtil.GetCurrentSEATime();

            _unitOfWork.GetRepository<Product>().UpdateAsync(product);
            await _unitOfWork.CommitAsync();

            return new BaseResponse<bool>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Xóa sản phẩm thành công",
                data = true
            };
        }

        public async Task<BaseResponse<IPaginate<GetProductResponse>>> GetAllProduct(int page, int size)
        {
            var products = await _unitOfWork.GetRepository<Product>().GetPagingListAsync(
                selector: p => new GetProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Brand = p.Brand,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    Color = p.Color,
                    Size = p.Size,
                    AgeRange = p.AgeRange,
                    Image = p.Image,
                    Status = p.Status,
                    CategoryId = p.CategoryId
                },
                predicate: p => p.Active,
                page: page,
                size: size
                );

            return new BaseResponse<IPaginate<GetProductResponse>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Lấy danh sách sản phẩm thành công",
                data = products
            };
        }

        public async Task<BaseResponse<GetProductResponse>> GetProductById(Guid id)
        {
            var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
                selector: p => new GetProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Brand = p.Brand,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    Color = p.Color,
                    Size = p.Size,
                    AgeRange = p.AgeRange,
                    Image = p.Image,
                    Status = p.Status,
                    CategoryId = p.CategoryId
                },
                predicate: p => p.Id.Equals(id) && p.Active);

            if (product == null)
            {
                throw new NotFoundException("Không tìm thấy sản phẩm");
            }


            return new BaseResponse<GetProductResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Lấy thông tin sản phẩm thành công",
                data = product
            };
        }

        public async Task<BaseResponse<GetProductResponse>> UpdateProduct(Guid id, UpdateProductRequest request)
        {
            var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
                predicate: p => p.Id.Equals(id) && p.Active);

            if (product == null)
            {
                throw new NotFoundException("Không tìm thấy sản phẩm");
            }

            if (request.CategoryId.HasValue)
            {
                var productCategory = await _unitOfWork.GetRepository<ProductCategory>().SingleOrDefaultAsync(
                    predicate: pc => pc.Id.Equals(request.CategoryId));
                if (productCategory == null)
                {
                    throw new NotFoundException("Không tìm thấy danh mục sản phẩm");
                }
            }

            product.Name = request.Name ?? product.Name;
            product.Description = request.Description ?? product.Description;
            product.Brand = request.Brand ?? product.Brand;
            product.Price = request.Price ?? product.Price;
            product.Quantity = request.Quantity ?? product.Quantity;
            product.Color = request.Color ?? product.Color;
            product.Size = request.Size ?? product.Size;
            product.AgeRange = request.AgeRange ?? product.AgeRange;
            product.Status = request.Status.GetDescriptionFromEnum() ?? product.Status;
            product.CategoryId = request.CategoryId ?? product.CategoryId;

            _unitOfWork.GetRepository<Product>().UpdateAsync(product);

            await _unitOfWork.CommitAsync();

            return new BaseResponse<GetProductResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Cập nhật sản phẩm thành công",
                data = new GetProductResponse
                {
                    Id = product.Id,
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
                    CategoryId = product.CategoryId,
                }
            };
        }
    }
}
