using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Enum;

namespace OhBau.Model.Payload.Response.Blog
{
    public class GetBlog
    {
        public Guid Id { get; set; }
        public string Title {  get; set; }
        public string Content { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public BlogStatusEnum? Status { get; set; }
        public string? ReasonReject {  get; set; }
        public string? Email {  get; set; }
        public bool IsDelete {  get; set; }
        public DateTime? DeletedDate { get; set; }

        public int TotalLike {  get; set; }
    }
}
