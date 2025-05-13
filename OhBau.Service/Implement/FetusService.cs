using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Enum;
using OhBau.Model.Exception;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Fetus;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Fetus;
using OhBau.Model.Utils;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class FetusService : BaseService<FetusService>, IFetusService
    {
        private readonly IMemoryCache _cache;
        private readonly GenericCacheInvalidator<Fetus> _fetusCacheInvalidator;
        public FetusService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<FetusService> logger, 
            IMapper mapper, IHttpContextAccessor httpContextAccessor, GenericCacheInvalidator<Fetus> fetusCacheInvalidator,
            IMemoryCache cache) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _fetusCacheInvalidator = fetusCacheInvalidator;
            _cache = cache;
        }

        public async Task<BaseResponse<CreateFetusResponse>> CreateFetus(CreateFetusRequest request)
        {
            Guid? userId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            if (userId == null)
            {
                return new BaseResponse<CreateFetusResponse>
                {
                    status = StatusCodes.Status400BadRequest.ToString(),
                    message = "Không thể xác định người dùng",
                    data = null
                };
            }

            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: a => a.Id.Equals(userId) && a.Active == true);
            if (account == null)
            {
                return new BaseResponse<CreateFetusResponse>
                {
                    status = StatusCodes.Status404NotFound.ToString(),
                    message = "Tài khoản không tồn tại",
                    data = null
                };
            }

            if (string.IsNullOrEmpty(account.Role))
            {
                return new BaseResponse<CreateFetusResponse>
                {
                    status = StatusCodes.Status400BadRequest.ToString(),
                    message = "Vai trò không hợp lệ",
                    data = null
                };
            }

            var currentParent = await _unitOfWork.GetRepository<Parent>().SingleOrDefaultAsync(
                predicate: p => p.AccountId == userId && p.Active == true);
            if (currentParent == null)
            {
                return new BaseResponse<CreateFetusResponse>
                {
                    status = StatusCodes.Status404NotFound.ToString(),
                    message = "Hồ sơ người dùng không tồn tại",
                    data = null
                };
            }

            bool isFather = account.Role.Equals(RoleEnum.FATHER.GetDescriptionFromEnum());

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var fetus = _mapper.Map<Fetus>(request);
                await _unitOfWork.GetRepository<Fetus>().InsertAsync(fetus);

                var currentParentRelation = new ParentRelation
                {
                    Id = Guid.NewGuid(),
                    AccountId = userId,
                    ParentId = currentParent.Id,
                    FetusId = fetus.Id,
                    RelationType = "Fetus",
                    Active = true,
                    CreateAt = TimeUtil.GetCurrentSEATime(),
                    UpdateAt = TimeUtil.GetCurrentSEATime(),
                    DeleteAt = null
                };
                await _unitOfWork.GetRepository<ParentRelation>().InsertAsync(currentParentRelation);

                if (isFather)
                {
                    var motherRelation = await _unitOfWork.GetRepository<ParentRelation>().SingleOrDefaultAsync(
                        predicate: m => m.AccountId.Equals(userId) && m.RelationType.Equals("Mother") && m.FetusId == null && m.Active == true);
                    if (motherRelation == null)
                    {
                        return new BaseResponse<CreateFetusResponse>
                        {
                            status = StatusCodes.Status404NotFound.ToString(),
                            message = "Hồ sơ mẹ không tồn tại",
                            data = null
                        };
                    }

                    var mother = await _unitOfWork.GetRepository<Parent>().SingleOrDefaultAsync(
                        predicate: p => p.Id.Equals(motherRelation.ParentId) && p.Active == true);
                    if (mother == null)
                    {
                        return new BaseResponse<CreateFetusResponse>
                        {
                            status = StatusCodes.Status404NotFound.ToString(),
                            message = "Hồ sơ mẹ không tồn tại",
                            data = null
                        };
                    }

                    var motherFetusRelation = new ParentRelation
                    {
                        Id = Guid.NewGuid(),
                        AccountId = null,
                        ParentId = mother.Id,
                        FetusId = fetus.Id,
                        RelationType = "Fetus",
                        Active = true,
                        CreateAt = TimeUtil.GetCurrentSEATime(),
                        UpdateAt = TimeUtil.GetCurrentSEATime(),
                        DeleteAt = null
                    };
                    await _unitOfWork.GetRepository<ParentRelation>().InsertAsync(motherFetusRelation);
                }

                await _unitOfWork.CommitTransactionAsync();

                _fetusCacheInvalidator.InvalidateEntityList();
                _fetusCacheInvalidator.InvalidateEntity(fetus.Id);

                return new BaseResponse<CreateFetusResponse>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Tạo thai nhi thành công",
                    data = _mapper.Map<CreateFetusResponse>(fetus)
                };
            }
            catch (Exception ex)
            {
                try
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, "Failed to rollback transaction for CreateFetus: {OriginalError}", ex.Message);
                }
                return new BaseResponse<CreateFetusResponse>
                {
                    status = StatusCodes.Status400BadRequest.ToString(),
                    message = $"Tạo thai nhi thất bại: {ex.Message}",
                    data = null
                };
            }
        }

        public async Task<BaseResponse<bool>> DeleteFetus(Guid id)
        {
            var fetus = await _unitOfWork.GetRepository<Fetus>().SingleOrDefaultAsync(
                predicate: f => f.Id.Equals(id) && f.Active == true);

            if (fetus == null)
            {
                throw new NotFoundException("Không tìm thấy thai nhi");
            }

            fetus.Active = false;
            _unitOfWork.GetRepository<Fetus>().UpdateAsync(fetus);
            await _unitOfWork.CommitAsync();

            _fetusCacheInvalidator.InvalidateEntityList();
            _fetusCacheInvalidator.InvalidateEntity(fetus.Id);

            return new BaseResponse<bool>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Xóa thai nhi thành công",
                data = true
            };
        }

        public async Task<BaseResponse<IPaginate<GetFetusResponse>>> GetAllFetus(int page, int size)
        {
            var listParameter = new ListParameters<Fetus>(page, size);
            var cacheKey = _fetusCacheInvalidator.GetCacheKeyForList(listParameter);
            
            if (_cache.TryGetValue(cacheKey, out Paginate<GetFetusResponse> GetFetus))
            {

                return new BaseResponse<IPaginate<GetFetusResponse>>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "List fetus",
                    data = GetFetus
                };
            }

            var responses = await _unitOfWork.GetRepository<Fetus>().GetPagingListAsync(
                selector: f => _mapper.Map<GetFetusResponse>(f),
                predicate: f => f.Active == true,
                page: page,
                size: size);


            return new BaseResponse<IPaginate<GetFetusResponse>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "List fetus",
                data = responses
            };
        }

        public async Task<BaseResponse<GetFetusResponse>> GetFetusByCode(string code)
        {
            var response = await _unitOfWork.GetRepository<Fetus>().SingleOrDefaultAsync(
                selector : f => _mapper.Map<GetFetusResponse>(f),
                predicate: f => f.Code.Equals(code) && f.Active == true);

            if (response == null)
            {
                throw new NotFoundException("Không tìm thấy fetus");
            }

            return new BaseResponse<GetFetusResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Tìm thấy fetus",
                data = response
            };
        }

        public async Task<BaseResponse<GetFetusResponse>> GetFetusById(Guid id)
        {
            var cacheKey = _fetusCacheInvalidator.GetEntityCache<GetFetusResponse>(id);
            
            if (cacheKey != null)
            {
                return new BaseResponse<GetFetusResponse>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Tìm thấy fetus",
                    data = cacheKey
                };
            }
            var response = await _unitOfWork.GetRepository<Fetus>().SingleOrDefaultAsync(
                selector: f => _mapper.Map<GetFetusResponse>(f),
                predicate: f => f.Id.Equals(id) && f.Active == true);

            if (response == null)
            {
                throw new NotFoundException("Không tìm thấy fetus");
            }

            return new BaseResponse<GetFetusResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Tìm thấy fetus",
                data = response
            };
        }
    }
}
