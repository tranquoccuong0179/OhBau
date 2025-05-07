using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.Course
{
    public class GetCoursesResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public double Rating { get; set; }

        public long Duration { get; set; }

        public double Price { get; set; }

        public string Category { get; set; }

        public bool? Active { get; set; }

        public DateTime? CreateAt { get; set; }

        public DateTime? UpdateAt { get; set; }

        public DateTime? DeleteAt { get; set; }
    }
}
