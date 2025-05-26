using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Topic
{
    public class CreateTopicRequest
    {
        public string Title {  get; set; }

        public string Description {  get; set; }

        [Range(1,82000, ErrorMessage = "Duration must not be less than 1 and greater than 82000")]
        public long Duration {  get; set; }

        public Guid CourseId { get; set; }
    }
}
