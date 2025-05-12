using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.Comment
{
    public class GetComments
    {
        public Guid Id { get; set; }

        public string Comment { get; set; }

        public string Email { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get;set; }
        public List<GetComments> Replies { get; set; } = new List<GetComments>();


    }
}
