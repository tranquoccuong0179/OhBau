using System;
using System.Collections.Generic;

namespace OhBau.Model.Entity;

public partial class MotherHealthRecord
{
    public Guid Id { get; set; }

    public Guid ParentId { get; set; }

    public double Weight { get; set; }

    public double BloodPressure { get; set; }

    public bool? Active { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual Parent Parent { get; set; } = null!;
}
