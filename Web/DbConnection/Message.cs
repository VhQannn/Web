using System;
using System.Collections.Generic;

namespace Web.dbConnection;

public partial class Message
{
    public int MessageId { get; set; }

    public int ConversationId { get; set; }

    public int SenderId { get; set; }

    public int? ReceiverId { get; set; }

    public string? MessageText { get; set; }

    public DateTime? SentTime { get; set; }

    public string? MessageType { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual ICollection<MessageReadStatus> MessageReadStatuses { get; set; } = new List<MessageReadStatus>();

    public virtual ICollection<Multimedium> Multimedia { get; set; } = new List<Multimedium>();

    public virtual User Sender { get; set; } = null!;
}
