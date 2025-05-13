using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Blog
{
    public class UpdateBlogRequest
    {
        public string? Title { get; set; }

        public string? Content { get; set; }
    }
}
