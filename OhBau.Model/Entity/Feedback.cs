using System;
using System.Collections.Generic;

namespace OhBau.Model.Entity;

public partial class Feedback
{
    public Guid Id { get; set; }

    public double Rating { get; set; }

    public string Content { get; set; } = null!;

    public Guid? BookingId { get; set; }

    public Guid? DoctorId { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Doctor? Doctor { get; set; }
}
