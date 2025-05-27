using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.Feedback
{
    public class GetFeedbackByDoctorId
    {
        public Guid Id { get; set; }

        public string? FullName { get; set; }

        public List<GetFeedback>? Feedbacks { get; set; }
    }

    public class GetFeedback
    {
        public Guid Id { get; set; }

        public double Rating { get; set; }

        public string? Content { get; set; }
    }
}
