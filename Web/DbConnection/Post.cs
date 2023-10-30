using System;
using System.Collections.Generic;

namespace Web.DbConnection;

public partial class Post
{
    public int PostId { get; set; }

    public int? UserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public DateTime PostDate { get; set; }

    public string? TimeSlot { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual User? User { get; set; }
}
