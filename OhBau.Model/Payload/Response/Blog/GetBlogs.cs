﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Payload.Response.LikeBlog;

namespace OhBau.Model.Payload.Response.Blog
{
    public class GetBlogs
    {
        public Guid Id { get; set; }

        public string Title {  get; set; }

        public string Content { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int TotalComment {  get; set; }
        public int TotalLike {  get; set; }
        public string AuthorEmail { get; set; }
        public Guid AuthorId { get; set; }
        public List<LikeBlogs> LikeBlogs { get; set; }
        

    }
}
