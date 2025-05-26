using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Entity
{
    public class Topic
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDelete { get; set; }
        public Guid CourseId { get; set; }
        public virtual Course Course { get; set; }

        public ICollection<Chapter> Chapters { get; set; }
    }
}
