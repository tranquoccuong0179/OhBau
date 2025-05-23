using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Enum;

namespace OhBau.Model.Payload.Response.Order
{
    public class GetOrders
    {
        public Guid Id { get; set; }
        public double TotalPrice {  get; set; }
        public DateTime CreatedDate { get; set; }
        public PaymentStatusEnum PaymentStatus { get; set; }
        public string Email {  get; set; }
        public string Phone {  get; set; }
    }
}
