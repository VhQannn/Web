using System;
using System.Collections.Generic;

namespace Web.DbConnection;

public partial class QuestionTemplatesDetail
{
    public int QuestionTemplatesDetailId { get; set; }

    public int QuestionTemplateId { get; set; }

    public int QId { get; set; }

    public string QText { get; set; } = null!;

    public virtual ICollection<Multimedium> Multimedia { get; set; } = new List<Multimedium>();

    public virtual QuestionTemplate QuestionTemplate { get; set; } = null!;

    public virtual ICollection<QuestionTemplateDetailQaid> QuestionTemplateDetailQaids { get; set; } = new List<QuestionTemplateDetailQaid>();
}
