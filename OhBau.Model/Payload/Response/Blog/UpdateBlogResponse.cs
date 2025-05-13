using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Enum;

namespace OhBau.Model.Payload.Response.Blog
{
    public class UpdateBlogResponse
    {
        public Guid? Id { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public BlogStatusEnum? Status { get; set; }
    }
}
