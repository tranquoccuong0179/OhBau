using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.Slot
{
    public class GetSlotResponse
    {
        public string? Name { get; set; } = null!;

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }
    }
}
