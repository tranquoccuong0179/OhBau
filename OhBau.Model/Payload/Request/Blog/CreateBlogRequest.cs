using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Enum;

namespace OhBau.Model.Payload.Request.Blog
{
    public class CreateBlogRequest
    {
        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

    }
}
