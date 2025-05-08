using System;
using System.Collections.Generic;

namespace OhBau.Model.Entity;

public partial class Doctor
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public string Gender { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Address { get; set; } = null!;

    public Guid MajorId { get; set; }

    public Guid AccountId { get; set; }

    public bool? Active { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public string? Avatar { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<DoctorSlot> DoctorSlots { get; set; } = new List<DoctorSlot>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual Major Major { get; set; } = null!;
}
