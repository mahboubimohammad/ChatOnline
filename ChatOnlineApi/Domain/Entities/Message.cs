using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChatOnlineApi.Domain.Entities;

[Index("ConversationId", Name = "IX_Messages_ConversationId")]
[Index("ConversationId", "CreatedAt", Name = "IX_Messages_ConversationId_CreatedAt")]
[Index("ConversationId", "CreatedAt", Name = "IX_Messages_Conversation_CreatedAt", IsDescending = new[] { false, true })]
public partial class Message
{
    [Key]
    public int Id { get; set; }

    public int ConversationId { get; set; }

    public int SenderId { get; set; }

    public string Text { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    public int? ReplyToMessageId { get; set; }

    public int MessageTypeId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DeletedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EditedAt { get; set; }

    [InverseProperty("ReplyToMessage")]
    public virtual ICollection<Message> InverseReplyToMessage { get; set; } = new List<Message>();

    [InverseProperty("Message")]
    public virtual ICollection<MessageAttachment> MessageAttachments { get; set; } = new List<MessageAttachment>();

    [InverseProperty("Message")]
    public virtual ICollection<MessageReaction> MessageReactions { get; set; } = new List<MessageReaction>();

    [InverseProperty("Message")]
    public virtual ICollection<MessageReadReceipt> MessageReadReceipts { get; set; } = new List<MessageReadReceipt>();

    [ForeignKey("MessageTypeId")]
    [InverseProperty("Messages")]
    public virtual MessageType MessageType { get; set; } = null!;

    [InverseProperty("Message")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [ForeignKey("ReplyToMessageId")]
    [InverseProperty("InverseReplyToMessage")]
    public virtual Message? ReplyToMessage { get; set; }
}
