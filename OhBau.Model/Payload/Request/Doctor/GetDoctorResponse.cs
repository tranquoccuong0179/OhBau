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
        public string? Avatar { get; set; } = null!;
        public Guid MajorId { get; set; }
        public string Major { get; set; } = null!;

        public DateOnly Dob { get; set; }

        public string Gender { get; set; } = null!;

        public string Content { get; set; } = null!;
            
        public string Address { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Phone { get; set; }

        public bool? Active { get; set; }

        public List<string> Experence {  get; set; } = null!;
        public List<string> Focus {  get; set; } = null!;

        public List<string> MedicalProfile { get; set; }
        public List<string> CareerPath { get; set; }

        public List<string> OutStanding {  get; set; }

        public int totalFeedbacks { get; set; }
        public double Rating {  get; set; }

        public string? WorkSchedule {  get; set; } = null!;

    }
}
