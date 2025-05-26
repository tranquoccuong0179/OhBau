using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.Booking
{
    public class CreateBookingResponse
    {
        public Guid Id { get; set; }

        public Guid ParentId { get; set; }

        public Guid DotorSlotId { get; set; }

        public string? Type { get; set; }

        public string? BookingModule { get; set; }

        public string? Description { get; set; }

        public string? FullName { get; set; }

        public int YearOld { get; set; }

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public DateOnly Date { get; set; }
    }
}
