using System;
using System.Collections.Generic;

namespace Web.DbConnection;

public partial class VirtualCurrency
{
    public int VirtualCurrencyId { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public DateTime TransactionDate { get; set; }

    public string? TransactionType { get; set; }

    public virtual User User { get; set; } = null!;
}
