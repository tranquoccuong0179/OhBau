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
        public long Id { get; set; }
        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }
        public PaymentTypeEnum Provider { get; set; }    
        public string PaymentUrl {  get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public PaymentStatusEnum Status { get; set; }
        public TransactionTypeEnum Type { get; set; }
    }
}
