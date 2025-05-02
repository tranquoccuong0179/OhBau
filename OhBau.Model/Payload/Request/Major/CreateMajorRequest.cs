using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Major
{
    public class CreateMajorRequest
    {
        [Required(ErrorMessage = "Major name is required")]

        public string Name { get; set; }

        public bool Active { get; set; } = true;

        public DateTime CreateAt { get; set; } = DateTime.Now;
    }
}
