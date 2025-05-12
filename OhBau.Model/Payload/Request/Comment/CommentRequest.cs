using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Comment
{
    public class CommentRequest
    {
        public Guid BlogId { get; set; }

        [Required(ErrorMessage = "Comment content is required")]
        public string Comment {  get; set; }

    }
}
