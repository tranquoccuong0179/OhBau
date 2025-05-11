using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPayService.Config
{
    
    public class VNPayConfig
    {
        public string ReturnUrl {  get; set; } 

        public string PaymentUrl { get; set; }

        public string TmnCode {  get; set; }

        public string SecretKey {  get; set; }
    }
}
