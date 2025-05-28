using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Course
{
    public class CreateCourseRequest
    {
        [Required(ErrorMessage = "Name can be not empty")]
        public string Name {  get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public float? Rating { get; set; } = 0;

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public long Duration { get; set; } = 0;

        [Required(ErrorMessage = "Please enter price")]
        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 1")]
        public float Price {  get; set; }

        [Required(ErrorMessage = "Please select a category")]
        public Guid CategoryId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public bool IsActive { get; set; } = true;

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DateTime CreateAt { get; set; } = DateTime.Now;

    }
}
