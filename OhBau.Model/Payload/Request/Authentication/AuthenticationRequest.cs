using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Authentication
{
    public class AuthenticationRequest
    {
        public string Phone { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
