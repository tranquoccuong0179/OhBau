using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.Authentication
{
    public class AuthenticationResponse
    {
        public Guid Id { get; set; }
        public string? Phone { get; set; }
        public string? Role { get; set; }
        public string? AccessToken { get; set; }
    }
}
