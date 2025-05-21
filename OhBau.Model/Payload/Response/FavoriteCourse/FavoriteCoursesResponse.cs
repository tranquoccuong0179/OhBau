using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.FavoriteCourse
{
    public class FavoriteCoursesResponse
    {
        public Guid CourseId { get; set; }

        public string Name {  get; set; }

        public long Duration {  get; set; }

        public string Category {  get; set; }

    }
}
