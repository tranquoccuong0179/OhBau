using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Payload.Response.MotherHealth;

namespace OhBau.Model.Payload.Response.Parent
{
    public class GetParentResponse
    {
        public Guid? Id { get; set; }
        public string? FullName { get; set; }
        public DateOnly? Dob { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public GetMotherHealthResponse? GetMotherHealthResponse { get; set; }
    }
}
