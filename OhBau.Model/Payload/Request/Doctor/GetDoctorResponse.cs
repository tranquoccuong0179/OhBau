using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Doctor
{
    public class GetDoctorResponse
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = null!;

        public Guid MajorId { get; set; }
        public string Major { get; set; } = null!;

        public DateOnly Dob { get; set; }

        public string Gender { get; set; } = null!;

        public string Content { get; set; } = null!;
            
        public string Address { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Phone { get; set; }

        public bool? Active { get; set; }
    }
}
