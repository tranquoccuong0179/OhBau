using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Enum;

namespace OhBau.Model.Entity
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public double TotalPrice { get; set; }
        public string OrderCode { get; set; }
        public Guid AccountId { get; set; }
        public virtual Account Account { get; set; }

        public PaymentStatusEnum PaymentStatus { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }

    }
}
