namespace Web.DbConnection;

public partial class Purchase
{
    public int PurchaseId { get; set; }

    public int? BuyerId { get; set; }

    public int? ToolId { get; set; }

    public DateTime? PurchaseDate { get; set; }

    public int Amount { get; set; }

    public virtual User? Buyer { get; set; }

    public virtual Tool? Tool { get; set; }
}
