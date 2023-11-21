using System;
using System.Collections.Generic;

namespace Web.DbConnection;

public partial class QuestionTemplate
{
    public int QuestionTemplateId { get; set; }

    public string QuestionTemplateCode { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public virtual ICollection<QuestionTemplatesDetail> QuestionTemplatesDetails { get; set; } = new List<QuestionTemplatesDetail>();
}
