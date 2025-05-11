using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Chapter
{
    public class UpdateChapterRequest
    {
        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string VideoUrl { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;

        public Guid CourseId { get; set; }

        public bool? Active { get; set; }

        public DateTime? CreateAt { get; set; }

        public DateTime? UpdateAt { get; set; }

        public DateTime? DeleteAt { get; set; }
    }
}
