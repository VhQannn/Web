using System;
using System.Collections.Generic;

namespace Web.DbConnection;

public partial class MarkReport
{
    public int MarkReportId { get; set; }

    public int UserId { get; set; }

    public double? MarkScore { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<Multimedium> Multimedia { get; set; } = new List<Multimedium>();

    public virtual User User { get; set; } = null!;
}
