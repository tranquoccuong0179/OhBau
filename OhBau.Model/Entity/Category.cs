using System;
using System.Collections.Generic;

namespace OhBau.Model.Entity;

public partial class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int? Active { get; set; }

    public int? CreateAt { get; set; }

    public int? UpdateAt { get; set; }

    public int? DeleteAt { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
