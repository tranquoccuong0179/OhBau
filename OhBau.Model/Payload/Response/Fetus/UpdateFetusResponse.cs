using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.Fetus
{
    public class UpdateFetusResponse
    {
        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? Name { get; set; }

        public string? Code { get; set; }
    }
}
