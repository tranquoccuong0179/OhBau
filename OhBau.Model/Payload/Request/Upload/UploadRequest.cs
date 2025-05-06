using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OhBau.Model.Payload.Request.Upload
{
    public class UploadRequest
    {
        public IFormFile file { get; set; }
    }
}
