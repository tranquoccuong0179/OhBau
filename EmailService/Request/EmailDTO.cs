using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.Request
{
    public class EmailDTO
    {
        public string Email { get; set; }

        public string Subject {  get; set; }

        public string Body { get; set; }
    }
}
