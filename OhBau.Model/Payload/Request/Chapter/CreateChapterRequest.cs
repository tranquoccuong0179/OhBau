using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Chapter
{
    public class CreateChapterRequest
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title {  get; set; }

        [Required(ErrorMessage ="Content is required")]
        public string Content {  get; set; }

        [Required(ErrorMessage = "Video is required")]
        public string VideoUrl {  get; set; }

        [Required(ErrorMessage = "Image is required")]
        public string ImageUrl { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public bool Active { get; set; } = false;

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DateTime CreateAt { get; set; } = DateTime.Now;

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DateTime UpdateAt { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DateTime DeleteAt { get; set; }

        public Guid TopicId { get; set; }

    }
}
