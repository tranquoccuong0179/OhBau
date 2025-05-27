using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OhBau.Model.Entity;

public partial class FetusDetail
{
    [Required]
    public Guid Id { get; set; }

    public int Weekly { get; set; }
    public double Weight { get; set; }
    public double Height { get; set; }
    public double Gsd { get; set; }

    public double Crl { get; set; }

    public double Bpd { get; set; }

    public double Fl { get; set; }

    public double Hc { get; set; }

    public double Ac { get; set; }

    public double Bpm {  get; set; }

    public double Movement { get; set; }
    public Guid FetusId { get; set; }

    public bool? Active { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual Fetus Fetus { get; set; } = null!;
}
