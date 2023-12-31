﻿using System;
using System.Collections.Generic;

namespace Web.DbConnection;

public partial class Post
{
    public int PostId { get; set; }

    public int? UserId { get; set; }

    public int? PostCategoryId { get; set; }

    public string PostTitle { get; set; } = null!;

    public string? PostContent { get; set; }

    public DateTime PostDate { get; set; }

    public string? TimeSlot { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<ParentComment> ParentComments { get; set; } = new List<ParentComment>();

    public virtual PostCategory? PostCategory { get; set; }

    public virtual User? User { get; set; }
}
