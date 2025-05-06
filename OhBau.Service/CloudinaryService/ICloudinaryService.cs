using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OhBau.Model.Payload.Response;

namespace OhBau.Service.CloudinaryService
{
    public interface ICloudinaryService
    {
        Task<BaseResponse<string>> Upload(IFormFile file);
    }
}
