﻿using System;
using System.Collections.Generic;

namespace Web.DbConnection;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public string Message { get; set; } = null!;

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? RedirectUrl { get; set; }

    public virtual User User { get; set; } = null!;
}
