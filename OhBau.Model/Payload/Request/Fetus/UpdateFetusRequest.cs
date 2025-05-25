using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Fetus
{
    public class UpdateFetusRequest
    {
        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? Name { get; set; }
    }
}
