using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Feedback
{
    public class CreateFeedbackRequest
    {
        public double Rating { get; set; }

        public string Content { get; set; } = null!;

        public Guid BookingId { get; set; }

        public Guid DoctorId { get; set; }
    }
}
