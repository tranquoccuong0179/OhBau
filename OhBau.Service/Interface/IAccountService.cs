using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Payload.Request;
using OhBau.Model.Payload.Response;

namespace OhBau.Service.Interface
{
    public interface IAccountService
    {
        Task<BaseResponse<RegisterResponse>> RegisterAccount(RegisterRequest request);
    }
}
