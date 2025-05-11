using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Enum;

namespace OhBau.Model.Entity
{
    public class Cart
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }
        public double TotalPrice {  get; set; }
        public Guid AccountId { get; set; }

        public virtual Account Account { get; set; }

        public ICollection<CartItems> CartItems { get; set; }
    }
}
