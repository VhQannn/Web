using System;
using System.Collections.Generic;

namespace Web.dbConnection;

public partial class WithdrawalRequest
{
    public int WithdrawalRequestId { get; set; }

    public int? PaymentId { get; set; }

    public int? SupporterId { get; set; }

    public DateTime? RequestDate { get; set; }

    public string? Status { get; set; }

    public string? Comments { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual User? Supporter { get; set; }
}
