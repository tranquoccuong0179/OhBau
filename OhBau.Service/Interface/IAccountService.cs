using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Account;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Account;
using OhBau.Model.Payload.Response.Parent;

namespace OhBau.Service.Interface
{
    public interface IAccountService
    {
        Task<BaseResponse<RegisterResponse>> RegisterAccount(RegisterRequest request);
        Task<BaseResponse<IPaginate<GetAccountResponse>>> GetAccounts(int page, int size);
        Task<BaseResponse<GetAccountResponse>> GetAccount(Guid id);
        Task<BaseResponse<GetAccountResponse>> UpdateAccount(UpdateAccountRequest request);
        Task<BaseResponse<bool>> DeleteAccount(Guid id);
        Task<BaseResponse<GetParentResponse>> GetAccountProfile();
    }
}
