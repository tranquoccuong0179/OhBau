using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.Feedback
{
    public class CreateFeedbackResponse
    {
        public double Rating { get; set; }

        public string? Content { get; set; }

        public Guid? BookingId { get; set; }

        public Guid? DoctorId { get; set; }
    }
}
