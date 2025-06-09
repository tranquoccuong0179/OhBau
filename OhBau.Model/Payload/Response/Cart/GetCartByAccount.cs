using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.Order
{
    public class GetCartItem
    {
        public Guid ItemId { get; set; }
        public string Name {  get; set; }
        public string ImageUrl {  get; set; }
        public string Description { get; set; }
        public string Color {  get; set; }
        public string Size {  get; set; }
        public double UnitPrice { get; set; }
    }

    public class GetCartByAccount
    {
        public Guid CartId { get; set; }
        public List<GetCartItem> cartItem { get; set; }
        public double TotalPrice { get; set; }

        public int TotalItem {  get; set; }
    }
}
