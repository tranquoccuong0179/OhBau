using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Topic
{
    public class EditTopicRequest
    {
        public string Title {  get; set; }
        public string Description {  get; set; }
        public long Duration {  get; set; }
    }
}
