using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChatOnlineApi.Domain.Entities;

[Index("UserId", Name = "IX_ConversationParticipants_UserId")]
public partial class ConversationParticipant
{
    [Key]
    public int Id { get; set; }

    public int ConversationId { get; set; }

    public int UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime JoinedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LeftAt { get; set; }

    public bool IsActive { get; set; }

    public int RoleId { get; set; }

    public bool IsMuted { get; set; }

    public bool IsPinned { get; set; }

    public bool IsArchived { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("ConversationParticipants")]
    public virtual ParticipantRole Role { get; set; } = null!;
}
