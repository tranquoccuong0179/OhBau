using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Entity
{
    public class FeedBacks
    {
       public Guid Id { get; set; }

       public float Rating {  get; set; }

       public string Content {  get; set; }

       public Guid BookingID { get; set; }

      [ForeignKey("BookingID")]
      public virtual Booking Booking { get; set; }
    }
}
