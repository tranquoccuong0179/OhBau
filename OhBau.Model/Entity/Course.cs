using System;
using System.Collections.Generic;

namespace OhBau.Model.Entity;

public partial class Course
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public double Rating { get; set; }

    public long Duration { get; set; }

    public double Price { get; set; }

    public Guid CategoryId { get; set; }

    public bool? Active { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<MyCourse> MyCourses { get; set; } = new List<MyCourse>();


}
