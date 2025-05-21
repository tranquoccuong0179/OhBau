using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Like
{
    public class RatingRequest
    {
        public Guid CourseId { get; set; }
        public float Rating {  get; set; }
        public string Description {  get; set; }
    }
}
