using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Category;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Category;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class CategoryService : BaseService<CategoryService>, ICategoryService
    {
        private readonly IMemoryCache _cache;
        private readonly GenericCacheInvalidator<Category> _categoryCacheInvalidator;
        public CategoryService(IUnitOfWork<OhBauContext> unitOfWork, 
            ILogger<CategoryService> logger, IMapper mapper, 
            IHttpContextAccessor httpContextAccessor, IMemoryCache cache, GenericCacheInvalidator<Category> categoryCacheInvalidator) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _cache = cache;
            _categoryCacheInvalidator = categoryCacheInvalidator;
        }

        public async Task<BaseResponse<string>> CreateCategory(CreateCategoryRequest request)
        {
            try
            {
                var checkAlready = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(predicate: x => x.Name.Equals(request.Name));
                if (checkAlready != null)
                {
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status208AlreadyReported.ToString(),
                        message = "Category already exist",
                        data = null
                    };
                }

                var createCategory = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Active = true,
                    CreateAt = DateTime.Now,
                    UpdateAt = null,
                    DeleteAt = null
                };

                await _unitOfWork.GetRepository<Category>().InsertAsync(createCategory);
                await _unitOfWork.CommitAsync();

                _categoryCacheInvalidator.InvalidateEntityList();

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Create category success",
                    data = null
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<BaseResponse<string>> DeleteCategory(Guid categoryId)
        {
            var checkCategory = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(predicate: x => x.Id == categoryId);
            if (checkCategory == null)
            {
                return new BaseResponse<string>
                {
                    status = StatusCodes.Status404NotFound.ToString(),
                    message = "Category not found",
                    data = null
                };
            }

            checkCategory.Active = false;
            checkCategory.DeleteAt = DateTime.UtcNow;
            _unitOfWork.GetRepository<Category>().UpdateAsync(checkCategory);
            await _unitOfWork.CommitAsync();

            _categoryCacheInvalidator.InvalidateEntityList();

            return new BaseResponse<string>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Delete category success",
                data = null
            };
        }

        public async Task<BaseResponse<string>> EditCategory(Guid categoryId, EditCategoryRequest request)
        {
            try
            {
                var checkUpdate = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(predicate: x => x.Id == categoryId);
                if (checkUpdate == null)
                {
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status404NotFound.ToString(),
                        message = "Category not found",
                        data = null
                    };
                }

                var checkAlreadyName = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(predicate: x => x.Name.ToLower().Equals(request.Name));
                if (checkAlreadyName != null)
                {
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status208AlreadyReported.ToString(),
                        message = "Category name already exist",
                        data = null
                    };
                }

                checkUpdate.Name = request.Name;
                checkUpdate.UpdateAt = DateTime.Now;
                checkUpdate.CreateAt = checkUpdate.CreateAt;
                checkUpdate.DeleteAt = checkUpdate.DeleteAt;
                checkUpdate.Active = checkUpdate.Active;

                _unitOfWork.GetRepository<Category>().UpdateAsync(checkUpdate);
                await _unitOfWork.CommitAsync();

                _categoryCacheInvalidator.InvalidateEntityList();

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Edit category success",
                    data = null
                };
            }
            catch (Exception ex) {

                throw new Exception(ex.ToString());
            }
        }

        public async Task<BaseResponse<Paginate<CategoryResponse>>> GetCategories(int pageNumber, int pageSize)
        {
            var listParameters = new ListParameters<Category>(pageNumber, pageSize);
            var cacheKey = _categoryCacheInvalidator.GetCacheKeyForList(listParameters);

            if (_cache.TryGetValue(cacheKey, out Paginate<CategoryResponse> cachedCategories))
            {
                return new BaseResponse<Paginate<CategoryResponse>>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get categories success (cache)",
                    data = cachedCategories
                };
            }

            var getCategories = await _unitOfWork.GetRepository<Category>().GetPagingListAsync(
                page: pageNumber,
                size: pageSize,
                predicate: x => x.Active == true);

            var mapItem = getCategories.Items.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Active = c.Active,
                CreateAt = c.CreateAt,
                UpdateAt = c.UpdateAt,
                DeleteAt = c.DeleteAt
            }).ToList();

            var pagedResponse = new Paginate<CategoryResponse>
            {
                Items = mapItem,
                Page = pageNumber,
                Size = pageSize,
                Total = getCategories.Total
            };

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };

            _cache.Set(cacheKey, pagedResponse, options);
            _categoryCacheInvalidator.AddToListCacheKeys(cacheKey);


            return new BaseResponse<Paginate<CategoryResponse>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Get categories success",
                data = pagedResponse
            };
        }
    }
}
