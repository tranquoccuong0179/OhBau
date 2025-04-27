using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response
{
    public class BaseResponse<T>
    {
        public string status { get; set; }
        public string? message { get; set; }
        public T? data { get; set; }
    }
}
