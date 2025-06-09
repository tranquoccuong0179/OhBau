using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.Order
{
    public class CreateOrderResponse
    {
        public string OrderCode { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<OrderItem> OrderItems { get; set; }
        public double TotalPrice {  get; set; }

    }

    public class OrderItem
    {
       public string ProductName {  get; set; }

       public int Quantity { get; set; }
       public double Price {  get; set; }
    };


}
