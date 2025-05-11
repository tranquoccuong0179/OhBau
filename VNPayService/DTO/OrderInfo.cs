using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPayService.DTO
{
    public class OrderInfo
    {
        public Guid OrderID { get; set; }
        public float Amount { get; set; }
        public int Status { get; set; }
        public string Des { get; set; }
        public DateTime CreatedDate { get; set; }
        public string locale { get; set; }

    }
}
