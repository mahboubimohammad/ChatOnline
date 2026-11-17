using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChatOnlineApi.Domain.Entities;

public partial class Conversation
{
    [Key]
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? SupportAgentId { get; set; }

    [StringLength(200)]
    public string? SupportAgentName { get; set; }

    [StringLength(200)]
    public string? CreatedByName { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastMessageAt { get; set; }

    public bool IsActive { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Conversation")]
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    [InverseProperty("Conversation")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [ForeignKey("SupportAgentId")]
    [InverseProperty("Conversations")]
    public virtual SupportAgent? SupportAgent { get; set; }
}
