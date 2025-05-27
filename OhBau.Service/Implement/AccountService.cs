using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Account;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Account;
using OhBau.Model.Payload.Response.Parent;
using OhBau.Model.Utils;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;
using OhBau.Model.Exception;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

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

            var parent = await _unitOfWork.GetRepository<Parent>().SingleOrDefaultAsync(
                predicate: p => p.AccountId.Equals(account.Id) && p.Active == true);
            if (parent == null)
            {
                return new BaseResponse<bool>()
                {
                    status = StatusCodes.Status404NotFound.ToString(),
                    message = "Không tìm thấy bố mẹ này",
                    data = false
                };
            }
            parent.Active = false;
            parent.DeleteAt = TimeUtil.GetCurrentSEATime();
            parent.UpdateAt = TimeUtil.GetCurrentSEATime();
            _unitOfWork.GetRepository<Parent>().UpdateAsync(parent);

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

        public async Task<BaseResponse<GetParentResponse>> GetAccountProfile()
        {

            try
            {
                Guid? id = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
                var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                    predicate: a => a.Id.Equals(id) && a.Active == true);

                if (account == null)
                {
                    throw new NotFoundException("Không tìm thấy tài khoản này");
                }

                var parent = await _unitOfWork.GetRepository<Parent>().SingleOrDefaultAsync(
                    predicate: p => p.AccountId.Equals(account.Id) && p.Active == true);

                if (parent == null)
                {
                    throw new NotFoundException("Không tìm thấy thông tin tài khoản của người dùng hiện tại");
                }

                var response = _mapper.Map<GetParentResponse>(parent);
                response.Email = account.Email;
                response.Phone = account.Phone;

                return new BaseResponse<GetParentResponse>()
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Lấy thông tin hồ sơ tài khoản thành công",
                    data = response
                };
            }catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponse<IPaginate<GetAccountResponse>>> GetAccounts(int page, int size)
        {
            if(page < 1 || size < 1)
            {
                throw new BadHttpRequestException("Page hoặc size không hợp lệ");
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
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var isEmailExist = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                    predicate: u => u.Email.Equals(request.Email));
                if (isEmailExist != null)
                {
                    throw new CustomValidationException("Email đã tồn tại");
                }

                var isPhoneExist = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                    predicate: u => u.Phone.Equals(request.Phone));
                if (isPhoneExist != null)
                {
                    throw new CustomValidationException("Số điện thoại đã tồn tại");
                }

                var account = _mapper.Map<Account>(request);
                await _unitOfWork.GetRepository<Account>().InsertAsync(account);

                var parent = _mapper.Map<Parent>(request.RegisterParentRequest);
                parent.AccountId = account.Id;

                //Create Account Order
                var createOrder = new Cart
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    AccountId = account.Id,
                    TotalPrice = 0
                };

                await _unitOfWork.GetRepository<Cart>().InsertAsync(createOrder);

                await _unitOfWork.CommitAsync();
                await _unitOfWork.GetRepository<Parent>().InsertAsync(parent);

                bool isSuccessfully = await _unitOfWork.CommitAsync() > 0;

                if (isSuccessfully)
                {
                    await _unitOfWork.CommitTransactionAsync();

                    return new BaseResponse<RegisterResponse>()
                    {
                        status = StatusCodes.Status200OK.ToString(),
                        message = "Đăng kí tài khoản thành công",
                        data = _mapper.Map<RegisterResponse>(account)
                    };
                }

                throw new Exception("Đăng kí tài khoản thất bại");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<BaseResponse<bool>> ChangePassword(ChangePasswordRequest request)
        {
            Guid? id = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: a => a.Id.Equals(id) && a.Active == true);

            if (account == null)
            {
                throw new NotFoundException("Không tìm thấy tài khoản");
            }

            if (!account.Password.Equals(PasswordUtil.HashPassword(request.OldPassword)))
            {
                throw new BadHttpRequestException("Mật khẩu cũ không khớp");
            }

            if (!request.NewPassword.Equals(request.ConfirmPassword))
            {
                throw new BadHttpRequestException("Mật khẩu xác nhận không khớp");
            }

            account.Password = PasswordUtil.HashPassword(request.NewPassword);

            _unitOfWork.GetRepository<Account>().UpdateAsync(account);
            await _unitOfWork.CommitAsync();

            return new BaseResponse<bool>()
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Thay đổi mật khẩu thành công",
                data = true
            };
        }

        public async Task<BaseResponse<GetAccountResponse>> UpdateAccount(UpdateAccountRequest request)
        {
            try
            {
                Guid? id = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
                var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                    predicate: a => a.Id.Equals(id) && a.Active == true);

                if (account == null)
                {
                    throw new NotFoundException("Không tìm thấy tài khoản");
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
                throw new Exception("Cập nhật tài khoản thất bại");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
