using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Enum;
using OhBau.Model.Exception;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Fetus;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Fetus;
using OhBau.Model.Payload.Response.FetusResponse;
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
            bool isMother = account.Role.Equals(RoleEnum.MOTHER.GetDescriptionFromEnum());

            if (!isFather && !isMother)
            {
                return new BaseResponse<CreateFetusResponse>
                {
                    status = StatusCodes.Status400BadRequest.ToString(),
                    message = "Vai trò không hợp lệ để tạo thai nhi",
                    data = null
                };
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var fetus = _mapper.Map<Fetus>(request);
                await _unitOfWork.GetRepository<Fetus>().InsertAsync(fetus);

                var fetusDetail = new FetusDetail
                {
                    Id = Guid.NewGuid(),
                    Weekly = 0,
                    Weight = 0,
                    Height = 0,
                    Bpm = 0,
                    Movement = 0,
                    Gsd = 0,
                    Crl = 0,
                    Bpd = 0,
                    Fl = 0,
                    Hc = 0,
                    Ac = 0,
                    FetusId = fetus.Id,
                    Active = true,
                    CreateAt = TimeUtil.GetCurrentSEATime(),
                    UpdateAt = TimeUtil.GetCurrentSEATime(),
                };
                await _unitOfWork.GetRepository<FetusDetail>().InsertAsync(fetusDetail);

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
                else if (isMother)
                {
                    var motherRelation = new ParentRelation
                    {
                        Id = Guid.NewGuid(),
                        AccountId = account.Id,
                        ParentId = currentParent.Id,
                        FetusId = fetus.Id,
                        RelationType = "Mother",
                        Active = true,
                        CreateAt = TimeUtil.GetCurrentSEATime(),
                        UpdateAt = TimeUtil.GetCurrentSEATime(),
                        DeleteAt = null
                    };

                    await _unitOfWork.GetRepository<ParentRelation>().InsertAsync(motherRelation);
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

            var fetusList = await _unitOfWork.GetRepository<Fetus>().GetPagingListAsync(
                predicate: f => f.Active == true,
                include: f => f.Include(f => f.FetusDetails),
                page: page,
                size: size);

            var responses = new Paginate<GetFetusResponse>
            {
                Items = fetusList.Items.Select(f =>
                {
                    f.FetusDetails = f.FetusDetails.OrderByDescending(fd => fd.CreateAt).ToList();
                    return _mapper.Map<GetFetusResponse>(f);
                }).ToList(),
                Page = fetusList.Page,
                Size = fetusList.Size,
                Total = fetusList.Total,
                TotalPages = fetusList.TotalPages
            };


            return new BaseResponse<IPaginate<GetFetusResponse>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "List fetus",
                data = responses
            };
        }

        public async Task<BaseResponse<GetFetusResponse>> GetFetusByCode(string code)
        {
            var fetus = await _unitOfWork.GetRepository<Fetus>().SingleOrDefaultAsync(
                predicate: f => f.Code.Equals(code) && f.Active == true,
                include: f => f.Include(f => f.FetusDetails));

            if (fetus == null)
            {
                throw new NotFoundException("Không tìm thấy fetus");
            }

            fetus.FetusDetails = fetus.FetusDetails.OrderByDescending(fd => fd.CreateAt).ToList();

            var response = _mapper.Map<GetFetusResponse>(fetus);

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
            var fetus = await _unitOfWork.GetRepository<Fetus>().SingleOrDefaultAsync(
                predicate: f => f.Id.Equals(id) && f.Active == true,
                include: f => f.Include(f => f.FetusDetails));

            if (fetus == null)
            {
                throw new NotFoundException("Không tìm thấy fetus");
            }

            fetus.FetusDetails = fetus.FetusDetails.OrderByDescending(fd => fd.CreateAt).ToList();

            var response = _mapper.Map<GetFetusResponse>(fetus);

            return new BaseResponse<GetFetusResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Tìm thấy fetus",
                data = response
            };
        }

        public async Task<BaseResponse<UpdateFetusResponse>> UpdateFetus(Guid id, UpdateFetusRequest request)
        {
            var fetus = await _unitOfWork.GetRepository<Fetus>().SingleOrDefaultAsync(
                predicate: f => f.Id.Equals(id) && f.Active == true);

            if (fetus == null)
            {
                throw new NotFoundException("Không tìm thấy thai nhi");
            }

            fetus.StartDate = request.StartDate.HasValue ? request.StartDate.Value : fetus.StartDate;
            fetus.EndDate = request.EndDate ?? fetus.EndDate;
            fetus.Name = request.Name ?? fetus.Name;

            _unitOfWork.GetRepository<Fetus>().UpdateAsync(fetus);
            await _unitOfWork.CommitAsync();

            return new BaseResponse<UpdateFetusResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Cập nhật thành công",
                data = _mapper.Map<UpdateFetusResponse>(fetus)
            };

        }

        public async Task<BaseResponse<GetFetusDetailResponse>> UpdateFetusDetail(Guid id, EditFetusInformationRequest request)
        {
            var fetus = await _unitOfWork.GetRepository<Fetus>().SingleOrDefaultAsync(
                predicate: f => f.Id.Equals(id) && f.Active == true);

            if (fetus == null)
            {
                throw new NotFoundException("Không tìm thấy thai nhi");
            }

            var fetusDetail = new FetusDetail
            {
                Id = Guid.NewGuid(),
                Weekly = request.Weekly,
                Weight = request.Weight,
                Height = request.Height,
                Bpm = request.Bpm,
                Movement = request.Movement,
                Gsd = request.Gsd,
                Crl = request.Crl,
                Bpd = request.Bpd,
                Fl = request.Fl,
                Hc = request.Hc,
                Ac = request.Ac,
                FetusId = id,
                Active = true,
                CreateAt = TimeUtil.GetCurrentSEATime(),
                UpdateAt = TimeUtil.GetCurrentSEATime(),
            };

            await _unitOfWork.GetRepository<FetusDetail>().InsertAsync(fetusDetail);
            await _unitOfWork.CommitAsync();

            return new BaseResponse<GetFetusDetailResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message =  "Cập nhật thành công",
                data = _mapper.Map<GetFetusDetailResponse>(fetusDetail)
            };
        }
    }
}
