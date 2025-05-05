using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Doctor
{
    public class GetDoctorsResponse
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = null!;

        public string Avatar { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string Major { get; set; } = null!;
    }
}
