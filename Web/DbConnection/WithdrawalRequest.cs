using System;
using System.Collections.Generic;

namespace Web.DbConnection;

public partial class WithdrawalRequest
{
    public int WithdrawalRequestId { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public DateTime RequestDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Comments { get; set; }

    public virtual User User { get; set; } = null!;
}
