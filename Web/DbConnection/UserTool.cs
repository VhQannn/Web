using System;
using System.Collections.Generic;

namespace Web.dbConnection;

public partial class UserTool
{
    public int UserToolId { get; set; }

    public int? ToolId { get; set; }

    public int? UserId { get; set; }

    public string KeyCode { get; set; } = null!;

    public virtual Tool? Tool { get; set; }

    public virtual User? User { get; set; }
}
