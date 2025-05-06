using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Major
{
    public class EditMajorRequest
    {
        [Required]
        public string MajorName { get; set; }
    }
}
