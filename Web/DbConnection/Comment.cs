using System;
using System.Collections.Generic;

namespace Web.DbConnection;

public partial class Comment
{
    public int CommentId { get; set; }

    public int? PostId { get; set; }

    public int? ParentCommentId { get; set; }

    public int? UserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CommentDate { get; set; }

    public virtual ICollection<Comment> InverseParentComment { get; set; } = new List<Comment>();

    public virtual Comment? ParentComment { get; set; }

    public virtual Post? Post { get; set; }

    public virtual User? User { get; set; }
}
