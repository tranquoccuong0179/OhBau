using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Exception;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Fetus;
using OhBau.Model.Payload.Response.Parent;
using OhBau.Model.Payload.Response.ParentRelation;
using OhBau.Model.Utils;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class ParentRelationService : BaseService<ParentRelationService>, IParentRelationService
    {
        private readonly IMemoryCache _cache;
        private readonly GenericCacheInvalidator<ParentRelation> _parentRelationCacheInvalidator;
        public ParentRelationService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<ParentRelationService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<GetParentRelationResponse>> GetParentRelation()
        {
            Guid? userId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);

            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: a => a.Id.Equals(userId) && a.Active == true);

            if (account == null)
            {
                throw new NotFoundException("Tài khoản không tồn tại");
            }

            var relations = await _unitOfWork.GetRepository<ParentRelation>()
                .GetListAsync(
                    predicate: pr => pr.AccountId == userId && pr.Active == true,
                    include: src => src
                        .Include(pr => pr.Parent)
                        .ThenInclude(p => p.MotherHealthRecords.Where(mhr => mhr.Active == true))
                        .Include(pr => pr.Fetus)
                );

            var responseData = new GetParentRelationResponse
            {
                Father = _mapper.Map<GetParentResponse>(
                    relations.FirstOrDefault(pr => pr.RelationType == "Father")?.Parent),
                Mother = _mapper.Map<GetParentResponse>(
                    relations.FirstOrDefault(pr => pr.RelationType == "Mother")?.Parent),
                Fetuses = _mapper.Map<List<GetFetusResponse>>(
                    relations.Where(pr => pr.RelationType == "Fetus").Select(pr => pr.Fetus).ToList())
            };

            return new BaseResponse<GetParentRelationResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Lấy thông tin gia đình thành công",
                data = responseData
            };
        }
    }
}
