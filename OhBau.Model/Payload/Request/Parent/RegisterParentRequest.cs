using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Parent
{
    public class RegisterParentRequest
    {
        public string FullName { get; set; } = null!;
        public DateOnly Dob { get; set; }
    }
}
