using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.Order
{
    public class CreateOrderResponse
    {
        public Guid OrderId { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public double TotalPrice {  get; set; }

    }

    public class OrderItem
    {
       public string Course {  get; set; }

       public double Price {  get; set; }
    };


}
