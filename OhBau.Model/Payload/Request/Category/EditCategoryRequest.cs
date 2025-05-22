using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Request.Category
{
    public class EditCategoryRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Active {  get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public DateTime DeleteAt { get; set; }
    }
}
