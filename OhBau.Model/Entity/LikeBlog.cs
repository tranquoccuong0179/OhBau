using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Entity
{
    public class LikeBlog
    {
        public Guid Id { get; set; }

        public Guid AccountID { get; set; }
        public virtual Account Account { get; set; }

        public Guid BlogId { get; set; }
        public virtual Blog Blog { get; set; }

        public DateTime CreatedDate { get; set; }


    }
}
