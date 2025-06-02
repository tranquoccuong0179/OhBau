using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.LikeBlog
{
    public class LikeBlogs
    {
        public Guid AccountId { get; set; }

        public bool isLiked {  get; set; }
    }
}
