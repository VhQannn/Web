using System;
using System.Collections.Generic;

namespace Web.dbConnection;

public partial class MessageReadStatus
{
    public int StatusId { get; set; }

    public int? MessageId { get; set; }

    public int? UserId { get; set; }

    public DateTime? ReadTime { get; set; }

    public virtual Message? Message { get; set; }

    public virtual User? User { get; set; }
}
