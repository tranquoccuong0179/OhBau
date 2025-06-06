﻿using System;
using System.Collections.Generic;

namespace OhBau.Model.Entity;

public partial class Fetus
{
    public Guid Id { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public bool? Active { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual ICollection<FetusDetail> FetusDetails { get; set; } = new List<FetusDetail>();

    public virtual ICollection<ParentRelation> ParentRelations { get; set; } = new List<ParentRelation>();
}
