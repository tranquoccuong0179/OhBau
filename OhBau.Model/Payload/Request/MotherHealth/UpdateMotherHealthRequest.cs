using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.MotherHealth
{
    public class UpdateMotherHealthRequest
    {
        public double? Weight { get; set; }

        public double? BloodPressure { get; set; }
    }
}
