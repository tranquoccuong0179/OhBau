using System;
using System.Collections.Generic;

namespace OhBau.Model.Entity;

public partial class DoctorSlot
{
    public Guid Id { get; set; }

    public Guid DoctorId { get; set; }

    public Guid SlotId { get; set; }

    public bool? Active { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Slot Slot { get; set; } = null!;
}
