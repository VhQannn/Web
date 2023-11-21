using System;
using System.Collections.Generic;

namespace Web.DbConnection;

public partial class Course
{
    public int CourseId { get; set; }

    public int? UserId { get; set; }

    public string CourseTitle { get; set; } = null!;

    public string? CourseDescription { get; set; }

    public string CourseraEmail { get; set; } = null!;

    public string CourseraPassword { get; set; } = null!;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual ICollection<Multimedium> Multimedia { get; set; } = new List<Multimedium>();

    public virtual User? User { get; set; }
}
