using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Entity
{
    public class CartItems
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Products { get; set; }

        public double UnitPrice {  get; set; }
        public int Quantity {  get; set; }

        public Guid CartId { get; set; }
        public virtual Cart Cart { get; set; }

    }
}
