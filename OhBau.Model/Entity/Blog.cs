using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using OhBau.Model.Enum;

namespace OhBau.Model.Entity
{
    public class Blog
    {
        public Guid Id { get; set; }

        public string Title {  get; set; }

        public string Content {  get; set; }

        public BlogStatusEnum Status { get; set; }
        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate {  get; set; }

        public DateTime? DeletedDate { get; set; }

        public bool isDelete {  get; set; }
        public string? ReasonReject {  get; set; }

        public Guid AccountId { get; set; }

        public virtual Account Account { get; set; }
    }
}
