﻿using System;
using System.Collections.Generic;

namespace OhBau.Model.Entity;

public partial class Favorite
{
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }

    public Guid CourseId { get; set; }

    public DateTime? CreateAt { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;
}
