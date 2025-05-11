using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Order
{
    public class CreateOrderRequest
    {
        public Guid CartId { get; set; }
        
    }
}
