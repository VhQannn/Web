using System;
using System.Collections.Generic;

namespace Web.DbConnection;

public partial class PostCategory
{
    public int PostCategoryId { get; set; }

    public string PostCategoryName { get; set; } = null!;

    public virtual ICollection<Multimedium> Multimedia { get; set; } = new List<Multimedium>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
