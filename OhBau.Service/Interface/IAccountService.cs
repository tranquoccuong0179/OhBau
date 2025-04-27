using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Account;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Account;

namespace OhBau.Service.Interface
{
    public interface IAccountService
    {
        Task<BaseResponse<RegisterResponse>> RegisterAccount(RegisterRequest request);
        Task<BaseResponse<IPaginate<GetAccountResponse>>> GetAccounts(int page, int size);
    }
}
