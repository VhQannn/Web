namespace Web.DbConnection;

public partial class Assignment
{
    public int AssignmentId { get; set; }

    public int? UserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? Deadline { get; set; }

    public string? Status { get; set; }

    public virtual User? User { get; set; }
}
