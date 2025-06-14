using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Account
{
    public class UpdateAccountRequest
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public DateOnly? Dob { get; set; }
    }
}
