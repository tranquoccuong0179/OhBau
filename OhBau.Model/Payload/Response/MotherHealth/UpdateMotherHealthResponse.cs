using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.MotherHealth
{
    public class UpdateMotherHealthResponse
    {
        public Guid? Id { get; set; }

        public Guid? ParentId { get; set; }

        public double? Weight { get; set; }

        public double? BloodPressure { get; set; }
    }
}
