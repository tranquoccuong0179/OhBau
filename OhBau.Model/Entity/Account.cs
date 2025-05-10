using System;
using System.Collections.Generic;

namespace OhBau.Model.Entity;

public partial class Account
{
    public Guid Id { get; set; }

    public string Phone { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Role { get; set; }

    public bool? Active { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<MyCourse> MyCourses { get; set; } = new List<MyCourse>();

    public virtual ICollection<ParentRelation> ParentRelations { get; set; } = new List<ParentRelation>();

    public virtual ICollection<Parent> Parents { get; set; } = new List<Parent>();
}
