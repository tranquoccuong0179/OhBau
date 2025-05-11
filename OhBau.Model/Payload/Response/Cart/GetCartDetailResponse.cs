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
        public Guid DetailId { get; set; }
        public string Name { get; set; } = null!;

        public double Rating { get; set; }

        public long Duration { get; set; }

        public double Price { get; set; }
    }
}
