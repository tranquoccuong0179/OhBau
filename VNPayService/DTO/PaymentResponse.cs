using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPayService.DTO
{
    public class PaymentResponse
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public Guid OrderId { get; set; }
        public long VnpayTransactionId { get; set; }
        public long Amount { get; set; }
        public string BankCode { get; set; }
        public string TerminalId {  get; set; }
    }
}
