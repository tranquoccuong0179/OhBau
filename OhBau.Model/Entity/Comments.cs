using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Entity
{
    public class Comments
    {
        public Guid Id { get; set; }

        public string Comment {  get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public bool isDelete {  get; set; }

        public Guid BlogId { get; set; }
        public virtual Blog Blog { get; set; }

        public Guid? ParentId { get; set; }

        public virtual Comments? Parent { get; set; }

        public Guid AccountId { get; set; }
        public virtual Account? Account { get; set; }

    }
}
