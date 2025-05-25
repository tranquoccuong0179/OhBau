using System;
using System.Collections.Generic;

namespace OhBau.Model.Entity;

public partial class Booking
{
    public Guid Id { get; set; }

    public Guid ParentId { get; set; }

    public Guid DotorSlotId { get; set; }

    public string Type { get; set; } = null!;

    public string BookingModule { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateOnly Date { get; set; }

    public bool? Active { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public string? FullName {  get; set; }

    public int YearOld {  get; set; }

    public string? Address {  get; set; }

    public string? Phone {  get; set; }

    public virtual DoctorSlot DotorSlot { get; set; } = null!;

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual Parent Parent { get; set; } = null!;
}
