using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.Order
{
    public class GetCartItem
    {
        public string Name {  get; set; }
        public double UnitPrice { get; set; }
    }

    public class GetCartByAccount
    {
        public Guid CardId { get; set; }
        public List<GetCartItem> cartItem { get; set; }
        public double TotalPrice { get; set; }
    }
}
