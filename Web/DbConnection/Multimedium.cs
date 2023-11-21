using System;
using System.Collections.Generic;

namespace Web.dbConnection;

public partial class Multimedium
{
    public int MultimediaId { get; set; }

    public int? UserId { get; set; }

    public int? QuestionTemplatesDetailId { get; set; }

    public int? MarkReportId { get; set; }

    public int? PostCategoryId { get; set; }

    public int? PostId { get; set; }

    public int? AssignmentId { get; set; }

    public int? CourseId { get; set; }

    public string MultimediaUrl { get; set; } = null!;

    public string MultimediaType { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? MessageId { get; set; }

    public virtual Assignment? Assignment { get; set; }

    public virtual Course? Course { get; set; }

    public virtual MarkReport? MarkReport { get; set; }

    public virtual Message? Message { get; set; }

    public virtual Post? Post { get; set; }

    public virtual PostCategory? PostCategory { get; set; }

    public virtual QuestionTemplatesDetail? QuestionTemplatesDetail { get; set; }

    public virtual User? User { get; set; }
}
