using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.Chapter
{
    public class GetChapter
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; } 

        public string VideoUrl { get; set; } 

        public string ImageUrl { get; set; } 

        public string Course { get; set; }

        public bool? Active { get; set; }

        public DateTime? CreateAt { get; set; }

        public DateTime? UpdateAt { get; set; }

        public DateTime? DeleteAt { get; set; }
    }
}
