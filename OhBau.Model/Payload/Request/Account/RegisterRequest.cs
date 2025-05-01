using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Enum;
using OhBau.Model.Payload.Request.Parent;

namespace OhBau.Model.Payload.Request.Account
{
    public class RegisterRequest
    {
        public string Phone { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public RoleEnum Role { get; set; }
        public RegisterParentRequest RegisterParentRequest { get; set; } = null!;
    }
}
