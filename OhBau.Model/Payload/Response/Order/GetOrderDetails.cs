using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Payload.Response.Cart;

namespace OhBau.Model.Payload.Response.Order
{
    public class GetOrderDetails
    {
        public Guid Id { get; set; }

        public string CourseName {  get; set; }

        public double? CourseRating {  get; set; }

        public long Duration {  get; set; }
        public double Price {  get; set; }
        public string CategoryName {  get; set; }

    }

    
}
