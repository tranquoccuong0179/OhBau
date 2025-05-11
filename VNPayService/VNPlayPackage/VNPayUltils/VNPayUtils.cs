using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace VNPayService.VNPlayPackage.VNPayUltils
{
    public class VNPayUtils
    {
        public static String HmacSHA512(string key, String inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }
        public static string GetIpAddress(HttpContext context)
        {
            try
            {
                var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

                if (!string.IsNullOrEmpty(forwardedHeader) && forwardedHeader.ToLower() != "unknown")
                {
                    // Nếu có nhiều IP (qua proxy), lấy cái đầu tiên
                    var ip = forwardedHeader.Split(',').FirstOrDefault();
                    if (!string.IsNullOrEmpty(ip) && ip.Length <= 45)
                        return ip.Trim();
                }

                return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            }
            catch (Exception ex)
            {
                return "Invalid IP: " + ex.Message;
            }
        }
    }
}
