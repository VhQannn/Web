using System;
using System.Collections.Generic;

namespace Web.DbConnection;

public partial class ParentComment
{
    public int ParentCommentId { get; set; }

    public int? PostId { get; set; }

    public int? UserId { get; set; }

    public string? Content { get; set; }

    public DateTime? CommentDate { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Post? Post { get; set; }

    public virtual User? User { get; set; }
}
