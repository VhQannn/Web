using System;
using System.Collections.Generic;

namespace Web.DbConnection;

public partial class Conversation
{
    public int ConversationId { get; set; }

    public int UserId { get; set; }

    public DateTime? CreatedTime { get; set; }

    public DateTime? UpdatedTime { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsArchived { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<ConversationMember> ConversationMembers { get; set; } = new List<ConversationMember>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual User User { get; set; } = null!;
}
