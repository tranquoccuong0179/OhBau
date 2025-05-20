using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Entity
{
    public class FavoriteCourses
    {
        public Guid CourseId { get; set; }

        public virtual Course Course { get; set; }

        public Guid AccountId { get; set; }

        public virtual Account Account { get; set; }
    }
}
