using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.Cart
{
    public class GetCartDetailResponse
    {
        public List<CartItemDetail> CartItems { get; set; }
        public double TotalPrice { get; set; }
    }

    public class CartItemDetail
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; } = null!;
        public string ImageUrl {  get; set; }
        public string Description { get; set; }
        public string Brand {  get; set; }
        public string Color {  get; set; }
        public string Size {  get; set; }
        public string AgeRange {  get; set; }
        public double Price { get; set; }
        public int Quantity {  get; set; }
        public double TotalPrice { get; set; }

    }
}
