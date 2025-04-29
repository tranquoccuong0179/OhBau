using System;
using System.Collections.Generic;

namespace OhBau.Model.Entity;

public partial class Parent
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public Guid AccountId { get; set; }

    public bool? Active { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Fetus> Fetus { get; set; } = new List<Fetus>();
}
