using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OhBau.Model.Enum;

namespace OhBau.Model.Payload.Request.Product
{
    public class CreateProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string AgeRange { get; set; }
        public IFormFile Image { get; set; }
        //public ProductStatusEnum Status { get; set; }
        public Guid CategoryId { get; set; }

    }
}
