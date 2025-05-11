using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Course
{
    public class UpdateCourse
    {   
        public string Name {  get; set; }
        [Range(1, 876000, ErrorMessage = "Duration must be between 1 and 876,000 hours.")]
        public long Duration {  get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 1")]
        public float Price {  get; set; }
        public Guid CategoryId {  get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public bool Active { get; set; }

        [JsonIgnore(Condition =JsonIgnoreCondition.Always)]
        public DateTime CreateAt { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DateTime UpdateAt { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DateTime DeleteAt { get; set; }    
    }
}
