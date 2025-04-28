using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Payload.Request.Authentication;
using OhBau.Model.Payload.Response.Authentication;
using OhBau.Model.Payload.Response;

namespace OhBau.Service.Interface
{
    public interface IAuthService
    {
        Task<BaseResponse<AuthenticationResponse>> Authenticate(AuthenticationRequest request);

    }
}
