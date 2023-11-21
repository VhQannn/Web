using System;
using System.Collections.Generic;

namespace Web.DbConnection;

public partial class QuestionTemplateDetailQaid
{
    public int QuestionTemplatesDetailQaidsId { get; set; }

    public int QuestionTemplatesDetailId { get; set; }

    public int QAid { get; set; }

    public virtual QuestionTemplatesDetail QuestionTemplatesDetail { get; set; } = null!;
}
