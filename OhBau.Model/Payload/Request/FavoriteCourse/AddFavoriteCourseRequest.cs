using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.FavoriteCourse
{
    public class AddFavoriteCourseRequest
    {
        [Required(ErrorMessage = "Course is reqruied")]
        public Guid CourseId { get; set; }
    }
}
