using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.Major
{
    public class GetMajors
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool? Active {  get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public DateTime? DeleteAt { get; set; }

    }
}
