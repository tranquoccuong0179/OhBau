using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.MiddleWareDTO
{
    public class VisitDTO
    {
        public int Count {  get; set; }

        public Dictionary<string, string> IPS { get; set; }
    }
}
