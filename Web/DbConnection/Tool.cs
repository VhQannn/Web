using System;
using System.Collections.Generic;

namespace Web.DbConnection;

public partial class Tool
{
    public int ToolId { get; set; }

    public int? SellerId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

    public virtual User? Seller { get; set; }
}
