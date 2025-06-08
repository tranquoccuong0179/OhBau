using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Entity
{
    public class OrderDetail
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }

        public Guid ProductId { get; set; }
        public virtual Product Products { get; set; }

        public double UnitPrice { get; set; }

        public int Quantity {  get; set; }
        public double TotalPrice {  get; set; }


    }
}
