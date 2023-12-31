﻿using System;
using System.Collections.Generic;

namespace Web.DbConnection;

public partial class Rating
{
    public int RatingId { get; set; }

    public int? RaterId { get; set; }

    public int? SupporterId { get; set; }

    public int? RatingValue { get; set; }

    public string? Comments { get; set; }

    public DateTime? RatingDate { get; set; }

    public virtual User? Rater { get; set; }

    public virtual User? Supporter { get; set; }
}
