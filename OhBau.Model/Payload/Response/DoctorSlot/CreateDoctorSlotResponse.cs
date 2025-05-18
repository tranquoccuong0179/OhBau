using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.DoctorSlot
{
    public class CreateDoctorSlotResponse
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public Guid SlotId { get; set; }
    }
}
