﻿using System;
using System.Collections.Generic;

namespace Web.DbConnection;

public partial class ToolCategory
{
    public int ToolCategoryId { get; set; }

    public string ToolCategoryName { get; set; } = null!;

    public virtual ICollection<Tool> Tools { get; set; } = new List<Tool>();
}
