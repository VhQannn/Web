namespace Web.DbConnection;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? UserId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public int? RelatedId { get; set; }

    public string? ServiceType { get; set; }

    public string? Status { get; set; }

    public virtual User? User { get; set; }
}
