using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChatOnlineApi.Domain.Entities;

[Index("UserId", "IsRead", "CreatedAt", Name = "IX_Notifications_UserId_IsRead_CreatedAt", IsDescending = new[] { false, false, true })]
public partial class Notification
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? ConversationId { get; set; }

    public int? MessageId { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    [StringLength(1000)]
    public string? Body { get; set; }

    [StringLength(50)]
    public string NotificationType { get; set; } = null!;

    public bool IsRead { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ReadAt { get; set; }

    [ForeignKey("ConversationId")]
    [InverseProperty("Notifications")]
    public virtual Conversation? Conversation { get; set; }

    [ForeignKey("MessageId")]
    [InverseProperty("Notifications")]
    public virtual Message? Message { get; set; }
}
