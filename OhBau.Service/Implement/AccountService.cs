using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Account;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Account;
using OhBau.Model.Utils;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class AccountService : BaseService<AccountService>, IAccountService
    {
        public AccountService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<AccountService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<bool>> DeleteAccount(Guid id)
        {
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: a => a.Id.Equals(id) && a.Active == true);

            if (account == null)
            {
                return new BaseResponse<bool>()
                {
                    status = StatusCodes.Status404NotFound.ToString(),
                    message = "Không tìm thấy tài khoản này",
                    data = false
                };
            }

            account.Active = false;
            account.DeleteAt = TimeUtil.GetCurrentSEATime();
            account.UpdateAt = TimeUtil.GetCurrentSEATime();
            _unitOfWork.GetRepository<Account>().UpdateAsync(account);

            await _unitOfWork.CommitAsync();

            return new BaseResponse<bool>()
            {
                status = StatusCodes.Status404NotFound.ToString(),
                message = "Xóa tài khoản thành công",
                data = true
            };

        }

        public async Task<BaseResponse<GetAccountResponse>> GetAccount(Guid id)
        {
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                selector: a => _mapper.Map<GetAccountResponse>(a),
                predicate: a => a.Id.Equals(id) && a.Active == true);

            if (account == null)
            {
                return new BaseResponse<GetAccountResponse>()
                {
                    status = StatusCodes.Status404NotFound.ToString(),
                    message = "Không tìm thấy tài khoản này",
                    data = null
                };
            }

            return new BaseResponse<GetAccountResponse>()
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Tài khoản người dùng",
                data = account
            };
        }

        public async Task<BaseResponse<IPaginate<GetAccountResponse>>> GetAccounts(int page, int size)
        {
            if(page < 1 || size < 1)
            {
                return new BaseResponse<IPaginate<GetAccountResponse>>()
                {
                    status = StatusCodes.Status400BadRequest.ToString(),
                    message = "Page hoặc size không hợp lệ",
                    data = null
                };
            }

            var accounts = await _unitOfWork.GetRepository<Account>().GetPagingListAsync(
                selector: a => _mapper.Map<GetAccountResponse>(a),
                predicate: a => a.Active == true,
                orderBy: a => a.OrderByDescending(a => a.CreateAt),
                page: page,
                size: size);

            return new BaseResponse<IPaginate<GetAccountResponse>>()
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Danh sách tài khoản",
                data = accounts
            };
        }

        public async Task<BaseResponse<RegisterResponse>> RegisterAccount(RegisterRequest request)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(request.Email, emailPattern))
            {
                return new BaseResponse<RegisterResponse>()
                {
                    status = StatusCodes.Status400BadRequest.ToString(),
                    message = "Email không đúng định dạng",
                    data = null
                };
            }

            string phonePattern = @"^0\d{9}$";
            if (!Regex.IsMatch(request.Phone, phonePattern))
            {
                return new BaseResponse<RegisterResponse>()
                {
                    status = StatusCodes.Status400BadRequest.ToString(),
                    message = "Số điện thoại không đúng định dạng",
                    data = null
                };
            }

            var isEmailExist = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: u => u.Email.Equals(request.Email));
            if (isEmailExist != null)
            {
                return new BaseResponse<RegisterResponse>()
                {
                    status = StatusCodes.Status400BadRequest.ToString(),
                    message = "Email đã tồn tại",
                    data = null
                };
            }

            var isPhoneExist = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: u => u.Phone.Equals(request.Phone));
            if (isPhoneExist != null)
            {
                return new BaseResponse<RegisterResponse>()
                {
                    status = StatusCodes.Status400BadRequest.ToString(),
                    message = "Số điện thoại đã tồn tại",
                    data = null
                };
            }

            var account = _mapper.Map<Account>(request);
            await _unitOfWork.GetRepository<Account>().InsertAsync(account);
            bool isSuccessfully = await _unitOfWork.CommitAsync() > 0;

            if (isSuccessfully)
            {
                return new BaseResponse<RegisterResponse>()
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Đăng kí tài khoản thành công",
                    data = _mapper.Map<RegisterResponse>(account)
                };
            }

            return new BaseResponse<RegisterResponse>()
            {
                status = StatusCodes.Status400BadRequest.ToString(),
                message = "Đăng kí tài khoản thất bại",
                data = null
            };
        }

        public async Task<BaseResponse<GetAccountResponse>> UpdateAccount(UpdateAccountRequest request)
        {
            Guid? id = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: a => a.Id.Equals(id) && a.Active == true);

            if (account == null)
            {
                return new BaseResponse<GetAccountResponse>()
                {
                    status = StatusCodes.Status404NotFound.ToString(),
                    message = "Không tìm thấy tài khoản",
                    data = null
                };
            }

            account.Email = string.IsNullOrEmpty(request.Email) ? account.Email : request.Email;
            account.UpdateAt = TimeUtil.GetCurrentSEATime();

            _unitOfWork.GetRepository<Account>().UpdateAsync(account);
            bool isSuccessfully = await _unitOfWork.CommitAsync() > 0;

            if (isSuccessfully)
            {
                return new BaseResponse<GetAccountResponse>()
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Cập nhật tài khoản thành công",
                    data = _mapper.Map<GetAccountResponse>(account)
                };
            }

            return new BaseResponse<GetAccountResponse>()
            {
                status = StatusCodes.Status400BadRequest.ToString(),
                message = "Cập nhật tài khoản thất bại",
                data = null
            };
        }
    }
}
