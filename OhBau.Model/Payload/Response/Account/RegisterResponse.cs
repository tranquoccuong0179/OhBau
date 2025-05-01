using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Payload.Response.Parent;

namespace OhBau.Model.Payload.Response.Account
{
    public class RegisterResponse
    {
        public Guid Id { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public RegisterParentResponse? RegisterParentResponse { get; set; }
    }
}
