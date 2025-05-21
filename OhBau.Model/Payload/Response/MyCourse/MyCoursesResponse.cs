using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.MyCourse
{
    public class MyCoursesResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Rating {  get; set; }

        public long Duration {  get; set; }

    }
}
