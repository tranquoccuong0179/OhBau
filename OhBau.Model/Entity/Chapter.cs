using System;
using System.Collections.Generic;

namespace OhBau.Model.Entity;

public partial class Chapter
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string VideoUrl { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;

    public Guid CourseId { get; set; }

    public bool? Active { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual Course Course { get; set; } = null!;
}
