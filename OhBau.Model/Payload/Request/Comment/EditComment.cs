using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Comment
{
    public class EditComment
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public Guid CommentId { get; set; }

        public string Comment {  get; set; }
    }
}
