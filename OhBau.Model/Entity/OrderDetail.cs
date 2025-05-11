using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Entity
{
    public class OrderDetail
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }

        public Guid CourseId { get; set; }
        public virtual Course Course { get; set; }

        public double UnitPrice { get; set; }
    }
}
