using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Enum;

namespace OhBau.Model.Payload.Request.Booking
{
    public class CreateBookingRequest
    {
        public Guid DotorSlotId { get; set; }

        public string BookingModule { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string? FullName { get; set; }

        public int YearOld { get; set; }

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public DateOnly Date { get; set; }
    }
}
