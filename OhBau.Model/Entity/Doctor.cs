﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OhBau.Model.Entity;

public partial class Doctor
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public string Gender { get; set; } = null!;

    public string? Avatar {  get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Address { get; set; } = null!;

    public Guid MajorId { get; set; }

    public Guid AccountId { get; set; }

    public bool? Active { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public string? MedicalProfile { get; set; }

    public string? CareerPath { get; set; }

    public string? OutStanding { get; set; }

    public string? Experence { get; set; }

    public string? Focus { get; set; }

    public string? WorkSchedule { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<DoctorSlot> DoctorSlots { get; set; } = new List<DoctorSlot>();

    public virtual ICollection<Feedback>? Feedbacks { get; set; } = new List<Feedback>();
    public virtual Major Major { get; set; } = null!;
}
