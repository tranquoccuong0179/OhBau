using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Payload.Response.Slot;

namespace OhBau.Model.Payload.Response.DoctorSlot
{
    public class GetDoctorSlotResponse
    {
        public Guid Id { get; set; }
        public GetSlotResponse? GetSlot {  get; set; }
    }
}
