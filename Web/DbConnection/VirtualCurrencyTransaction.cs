using System;
using System.Collections.Generic;

namespace Web.DbConnection;

public partial class VirtualCurrencyTransaction
{
    public int TransactionId { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public DateTime TransactionDate { get; set; }

    public string? Description { get; set; }

    public virtual User User { get; set; } = null!;
}
