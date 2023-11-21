using System;
using System.Collections.Generic;

namespace Web.dbConnection;

public partial class ConversationMember
{
    public int ConversationMemberId { get; set; }

    public int ConversationId { get; set; }

    public int UserId { get; set; }

    public DateTime? JoinTime { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
