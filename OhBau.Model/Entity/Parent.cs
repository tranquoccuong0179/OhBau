using System;
using System.Collections.Generic;

namespace OhBau.Model.Entity;

public partial class Parent
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public Guid? AccountId { get; set; }

    public bool? Active { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<MotherHealthRecord> MotherHealthRecords { get; set; } = new List<MotherHealthRecord>();

    public virtual ICollection<ParentRelation> ParentRelations { get; set; } = new List<ParentRelation>();
}
