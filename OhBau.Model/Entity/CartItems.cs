using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Entity
{
    public class CartItems
    {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        public double UnitPrice {  get; set; }

        public Guid CartId { get; set; }
        public virtual Cart Cart { get; set; }

    }
}
