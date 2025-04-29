using System;
using System.Collections.Generic;

namespace OhBau.Model.Entity;

public partial class Fetu
{
    public Guid Id { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public double Weight { get; set; }

    public double Height { get; set; }

    public bool? Active { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual ICollection<FetusDetail> FetusDetails { get; set; } = new List<FetusDetail>();
}
