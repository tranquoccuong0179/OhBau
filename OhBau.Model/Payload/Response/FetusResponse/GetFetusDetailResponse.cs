using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.FetusResponse
{
    public class GetFetusDetailResponse
    {
        public Guid Id { get; set; }

        public int Weekly { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public double Gsd { get; set; }

        public double Crl { get; set; }

        public double Bpd { get; set; }

        public double Fl { get; set; }

        public double Hc { get; set; }

        public double Ac { get; set; }
    }
}
