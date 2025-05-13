using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Comment
{
    public class ReplyComment
    {
        [Required(ErrorMessage ="Reply content is required")]
        public string Comment { get; set; }

        public Guid ParentId { get; set; }

        public Guid BlogId { get; set; }

    }
}
