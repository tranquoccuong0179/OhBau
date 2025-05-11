using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Enum;

namespace OhBau.Model.Entity
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }
        public string Provider { get; set; }    
        public string Code { get; set; }    
        public double Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public PaymentStatusEnum Status { get; set; }
    }
}
