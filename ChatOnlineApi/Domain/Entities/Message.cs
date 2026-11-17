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

    [StringLength(200)]
    public string? SenderName { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ReadAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DeliveredAt { get; set; }

    public int? MessageReactionId { get; set; }

    [ForeignKey("ConversationId")]
    [InverseProperty("Messages")]
    public virtual Conversation Conversation { get; set; } = null!;

    [InverseProperty("ReplyToMessage")]
    public virtual ICollection<Message> InverseReplyToMessage { get; set; } = new List<Message>();

    [InverseProperty("Message")]
    public virtual ICollection<MessageAttachment> MessageAttachments { get; set; } = new List<MessageAttachment>();

    [ForeignKey("MessageReactionId")]
    [InverseProperty("Messages")]
    public virtual ReactionType? MessageReaction { get; set; }

    [ForeignKey("MessageTypeId")]
    [InverseProperty("Messages")]
    public virtual MessageType MessageType { get; set; } = null!;

    [InverseProperty("Message")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [ForeignKey("ReplyToMessageId")]
    [InverseProperty("InverseReplyToMessage")]
    public virtual Message? ReplyToMessage { get; set; }
}
