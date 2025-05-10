using System;
using System.Collections.Generic;

namespace OhBau.Model.Entity;

public partial class ParentRelation
{
    public Guid Id { get; set; }

    public Guid? AccountId { get; set; }

    public Guid? ParentId { get; set; }

    public Guid? FetusId { get; set; }

    public string? RelationType { get; set; }

    public bool? Active { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Fetus? Fetus { get; set; }

    public virtual Parent? Parent { get; set; }
}
