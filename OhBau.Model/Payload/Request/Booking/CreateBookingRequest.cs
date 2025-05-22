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

        public DateOnly Date { get; set; }
    }
}
