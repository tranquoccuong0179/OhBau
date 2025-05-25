using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Fetus
{
    public class EditFetusInformationRequest
    {
        [Range(1, 5200, ErrorMessage = "Weekly cannot be less than 1 and can be not greater than 5200.")]
        public int Weekly { get; set; }

        public double Weight { get; set; }
        public double Height { get; set; }

        [Range(0.1, 100, ErrorMessage = "GSD must be between 0.1 and 100 mm.")]
        public double Gsd { get; set; }

        [Range(0.1, 100, ErrorMessage = "CRL must be between 0.1 and 100 mm.")]
        public double Crl { get; set; }

        [Range(10, 120, ErrorMessage = "BPD must be between 10 and 120 mm.")]
        public double Bpd { get; set; }

        [Range(5, 80, ErrorMessage = "FL must be between 5 and 80 mm.")]
        public double Fl { get; set; }

        [Range(50, 400, ErrorMessage = "HC must be between 50 and 400 mm.")]
        public double Hc { get; set; }

        [Range(50, 400, ErrorMessage = "AC must be between 50 and 400 mm.")]
        public double Ac { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DateTime UpdateAt { get; set; } = DateTime.Now;
    }
}
