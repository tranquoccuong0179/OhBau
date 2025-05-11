using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNPayService.DTO;

namespace VNPayService
{
    public interface IVnPayService
    {
        Task<string> CreatePayment(CreateOrder request);
        Task<string> VnPayReturn();
        Task<PaymentResponse> ProcessVnPayReturn(Dictionary<string, string> queryParams);
    }
}
