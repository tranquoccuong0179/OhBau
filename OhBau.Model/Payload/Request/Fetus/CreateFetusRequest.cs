using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Fetus
{
    public class CreateFetusRequest
    {
        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? Name { get; set; }

        public double? Weight { get; set; }

        public double? Height { get; set; }
    }
}
