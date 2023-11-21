using System;
using System.Collections.Generic;

namespace Web.dbConnection;

public partial class Tool
{
    public int ToolId { get; set; }

    public int? ToolCategoryId { get; set; }

    public int? SellerId { get; set; }

    public string ToolName { get; set; } = null!;

    public string? ToolDescription { get; set; }

    public decimal ToolPrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

    public virtual User? Seller { get; set; }

    public virtual ToolCategory? ToolCategory { get; set; }

    public virtual ICollection<UserTool> UserTools { get; set; } = new List<UserTool>();
}
