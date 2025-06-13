using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.Product
{
    public class CreateProductResponse
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string AgeRange { get; set; }
        public string Image { get; set; }
        public string Status { get; set; }
        public Guid ProductCategoryId { get; set; }
    }
}
