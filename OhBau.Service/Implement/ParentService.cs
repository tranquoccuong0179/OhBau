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
using OhBau.Model.Payload.Request.Parent;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Parent;
using OhBau.Model.Utils;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class ParentService : BaseService<ParentService>, IParentService
    {
        public ParentService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<ParentService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<RegisterParentResponse>> AddNewParent(RegisterParentRequest request)
        {
            Guid? userId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: a => a.Id.Equals(userId) && a.Active == true);
            if(account == null)
            {
                throw new NotFoundException("Tài khoản không tồn tại");
            }

            if (string.IsNullOrEmpty(account.Role))
            {
                throw new BadHttpRequestException("Vai trò không hợp lệ");
            }

            var parent = await _unitOfWork.GetRepository<Parent>().SingleOrDefaultAsync(
                predicate: p => p.AccountId == userId && p.Active == true);
            if (parent == null)
            {
                throw new NotFoundException("Hồ sơ người dùng không tồn tại");
            }

            bool isFather = account.Role.Equals(RoleEnum.FATHER.GetDescriptionFromEnum());

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (isFather)
                {
                    var mother = _mapper.Map<Parent>(request);
                    await _unitOfWork.GetRepository<Parent>().InsertAsync(mother);

                    var fatherMotherRelation = new ParentRelation
                    {
                        Id = Guid.NewGuid(),
                        AccountId = userId,
                        ParentId = mother.Id,
                        RelationType = "Mother",
                        Active = true,
                        CreateAt = TimeUtil.GetCurrentSEATime(),
                        UpdateAt = TimeUtil.GetCurrentSEATime(),
                    };
                    await _unitOfWork.GetRepository<ParentRelation>().InsertAsync(fatherMotherRelation);

                    var motherRelation = new ParentRelation
                    {
                        Id = Guid.NewGuid(),
                        AccountId = null,
                        ParentId = mother.Id,
                        RelationType = "Mother",
                        Active = true,
                        CreateAt = TimeUtil.GetCurrentSEATime(),
                        UpdateAt = TimeUtil.GetCurrentSEATime(),
                    };
                    await _unitOfWork.GetRepository<ParentRelation>().InsertAsync(motherRelation);

                    var motherHelthRecord = new MotherHealthRecord
                    {
                        Id = Guid.NewGuid(),
                        ParentId = mother.Id,
                        Weight = 0,
                        BloodPressure = 0,
                        Active = true,
                        CreateAt = TimeUtil.GetCurrentSEATime(),
                        UpdateAt = TimeUtil.GetCurrentSEATime()
                    };
                    await _unitOfWork.GetRepository<MotherHealthRecord>().InsertAsync(motherHelthRecord);

                    var fatherSelfRelation = new ParentRelation
                    {
                        Id = Guid.NewGuid(),
                        AccountId = userId,
                        ParentId = parent.Id,
                        RelationType = "Father",
                        Active = true,
                        CreateAt = TimeUtil.GetCurrentSEATime(),
                        UpdateAt = TimeUtil.GetCurrentSEATime(),
                    };
                    await _unitOfWork.GetRepository<ParentRelation>().InsertAsync(fatherSelfRelation);
                }
                else if (!isFather)
                {
                    var motherRelation = new ParentRelation
                    {
                        Id = Guid.NewGuid(),
                        AccountId = userId,
                        ParentId = parent.Id,
                        RelationType = "Mother",
                        Active = true,
                        CreateAt = TimeUtil.GetCurrentSEATime(),
                        UpdateAt = TimeUtil.GetCurrentSEATime(),
                    };
                    await _unitOfWork.GetRepository<ParentRelation>().InsertAsync(motherRelation);

                    var motherHelthRecord = new MotherHealthRecord
                    {
                        Id = Guid.NewGuid(),
                        ParentId = motherRelation.Parent.Id,
                        Weight = 0,
                        BloodPressure = 0,
                        Active = true,
                        CreateAt = TimeUtil.GetCurrentSEATime(),
                        UpdateAt = TimeUtil.GetCurrentSEATime()
                    };
                    await _unitOfWork.GetRepository<MotherHealthRecord>().InsertAsync(motherHelthRecord);
                }

                await _unitOfWork.CommitTransactionAsync();
            }
            catch(Exception ex)
            {
                try
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, "Failed to rollback transaction for AddNewParent: {OriginalError}", ex.Message);
                }
                throw;
            }
            return new BaseResponse<RegisterParentResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Thêm hồ sơ thành công",
                data = _mapper.Map<RegisterParentResponse>(parent)
            };
        }
    }
}
