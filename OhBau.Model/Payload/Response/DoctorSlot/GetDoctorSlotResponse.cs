using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.DoctorSlot
{
    public class GetDoctorSlotResponse
    {
        public Guid Id { get; set; }
        public SlotResponse? Slot { get; set; }
        public bool IsBooking { get; set; }
    }

    public class GetDoctorSlotsForUserResponse
    {
        public string? Name { get; set; }
        public List<GetDoctorSlotResponse>? DoctorSlots { get; set; }
    }

    public class SlotResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
