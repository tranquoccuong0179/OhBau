using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Payload.Request.Doctor;
using OhBau.Model.Payload.Response.Parent;
using OhBau.Model.Payload.Response.Slot;

namespace OhBau.Model.Payload.Response.Booking
{
    public class GetBookingResponse
    {
        public GetParentResponse? Parent { get; set; }

        public string? Type { get; set; }

        public string? BookingModule { get; set; }

        public string? BookingCode { get; set; }

        public string? Description { get; set; }

        public DateOnly Date { get; set; }

        public DoctorResponse? Doctor { get; set; }

        public GetSlotResponse? Slot { get; set; }
    }

    public class DoctorResponse
    {
        public Guid Id { get; set; }

        public string? FullName { get; set; }

        public string? Avatar { get; set; }
    }
}
