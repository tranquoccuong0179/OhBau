using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Enum;
using OhBau.Model.Payload.Request.Authentication;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Authentication;
using OhBau.Model.Utils;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class AuthService : BaseService<AuthService>, IAuthService
    {
        public AuthService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<AuthService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<AuthenticationResponse>> Authenticate(AuthenticationRequest request)
        {
            Expression<Func<Account, bool>> searchFilter = p =>
                  p.Phone.Equals(request.Phone) &&
                  p.Password.Equals(PasswordUtil.HashPassword(request.Password)) &&
                  (p.Role == RoleEnum.ADMIN.GetDescriptionFromEnum() ||
                  p.Role == RoleEnum.FATHER.GetDescriptionFromEnum() ||
                  p.Role == RoleEnum.MOTHER.GetDescriptionFromEnum() ||
                  p.Role == RoleEnum.DOCTOR.GetDescriptionFromEnum() && p.Active == true);
            Account account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: searchFilter);
            if (account == null)
            {
                return new BaseResponse<AuthenticationResponse>()
                {
                    status = StatusCodes.Status400BadRequest.ToString(),
                    message = "Số điện thoại hoặc mật khẩu không đúng",
                    data = null
                };
            }

            RoleEnum role = EnumUtil.ParseEnum<RoleEnum>(account.Role);
            Tuple<string, Guid> guildClaim = new Tuple<string, Guid>("accountId", account.Id);
            var token = JwtUtil.GenerateJwtToken(account, guildClaim);

            var response = new AuthenticationResponse()
            {
                Id = account.Id,
                Phone = account.Phone,
                Role = account.Role,
                AccessToken = token,
            };

            return new BaseResponse<AuthenticationResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Đăng nhập thành công",
                data = response
            };
        }
    }
}
