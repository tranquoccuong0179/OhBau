using System;
using System.Collections.Generic;

namespace OhBau.Model.Entity;

public partial class Slot
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool? Active { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual ICollection<DoctorSlot> DoctorSlots { get; set; } = new List<DoctorSlot>();
}
