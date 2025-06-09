using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Exception;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.ProductCategory;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Product;
using OhBau.Model.Payload.Response.ProductCategory;
using OhBau.Model.Utils;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class ProductCategoryService : BaseService<ProductCategoryService>, IProductCategoryService
    {
        public ProductCategoryService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<ProductCategoryService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<CreateProductCategoryResponse>> CreaeteProductCategory(CreateProductCategoryRequest request)
        {
            var productCategoryExist = await _unitOfWork.GetRepository<ProductCategory>().SingleOrDefaultAsync(
                predicate: p => p.Name.Equals(request.Name));

            if (productCategoryExist != null)
            {
                throw new BadHttpRequestException("Danh mục sản phẩm đã tồn tại");
            }

            var productCategory = new ProductCategory
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                CreatedAt = TimeUtil.GetCurrentSEATime(),
                UpdatedAt = TimeUtil.GetCurrentSEATime(),
            };

            await _unitOfWork.GetRepository<ProductCategory>().InsertAsync(productCategory);

            await _unitOfWork.CommitAsync();

            return new BaseResponse<CreateProductCategoryResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Tạo danh mục sản phẩm thành công",
                data = new CreateProductCategoryResponse
                {
                    Name = productCategory.Name,
                    Description = productCategory.Description,
                }
            };
        }

        public async Task<BaseResponse<IPaginate<GetProductResponse>>> GetAllProductByCategory(Guid id, int page, int size)
        {
            var productCategory = await _unitOfWork.GetRepository<ProductCategory>().SingleOrDefaultAsync(
                predicate: pc => pc.Id.Equals(id));

            if (productCategory == null)
            {
                throw new NotFoundException("Danh mục sản phẩm không tồn tại");
            }

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
                predicate: p => p.CategoryId.Equals(id) && p.Active,
                page: page,
                size: size);


            return new BaseResponse<IPaginate<GetProductResponse>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Lấy danh sách sản phẩm theo danh mục thành công",
                data = products
            };
        }

        public async Task<BaseResponse<IPaginate<GetProductCategoryResponse>>> GetAllProductCategory(int page, int size)
        {
            var productCategories = await _unitOfWork.GetRepository<ProductCategory>().GetPagingListAsync(
                selector: p => new GetProductCategoryResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                },
                page: page,
                size: size);

            return new BaseResponse<IPaginate<GetProductCategoryResponse>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Lấy danh sách của danh mục sản phẩm thành công",
                data = productCategories
            };
        }

        public async Task<BaseResponse<GetProductCategoryResponse>> GetProductCategoryById(Guid id)
        {
            var productCategory = await _unitOfWork.GetRepository<ProductCategory>().SingleOrDefaultAsync(
                selector: p => new GetProductCategoryResponse
                {
                    Id = id,
                    Name = p.Name,
                    Description = p.Description,
                },
                predicate: p => p.Id.Equals(id));

            if (productCategory == null)
            {
                throw new NotFoundException("Không tìm thấy danh mục sản phẩm");
            }

            return new BaseResponse<GetProductCategoryResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Lấy danh mục sản phẩm thành công",
                data = productCategory
            };
        }

        public async Task<BaseResponse<GetProductCategoryResponse>> UpdateProductCategory(Guid id, UpdateProductCategoryRequest request)
        {
            var productCategory = await _unitOfWork.GetRepository<ProductCategory>().SingleOrDefaultAsync(
                predicate: p => p.Id.Equals(id));

            if (productCategory == null)
            {
                throw new NotFoundException("Không tìm thấy danh mục sản phẩm");
            }

            productCategory.Name = request.Name ?? productCategory.Name;
            productCategory.Description = request.Description ?? productCategory.Description;

            _unitOfWork.GetRepository<ProductCategory>().UpdateAsync(productCategory);
            await _unitOfWork.CommitAsync();

            return new BaseResponse<GetProductCategoryResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Cập nhật danh mục sản phẩm thành công",
                data = new GetProductCategoryResponse
                {
                    Id = id,
                    Name = productCategory.Name,
                    Description = productCategory.Description,
                }
            };
        }
    }
}
