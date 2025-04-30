using System;
using System.Collections.Generic;

namespace OhBau.Model.Entity;

public partial class Major
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public bool? Active { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
