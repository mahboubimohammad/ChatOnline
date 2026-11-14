using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChatOnlineApi.Domain.Entities;

[Index("MessageId", Name = "IX_MessageReactions_MessageId")]
[Index("MessageId", "UserId", "ReactionTypeId", Name = "UQ_MessageReactions_Message_User_Type", IsUnique = true)]
public partial class MessageReaction
{
    [Key]
    public int Id { get; set; }

    public int MessageId { get; set; }

    public int UserId { get; set; }

    public int ReactionTypeId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("MessageId")]
    [InverseProperty("MessageReactions")]
    public virtual Message Message { get; set; } = null!;

    [ForeignKey("ReactionTypeId")]
    [InverseProperty("MessageReactions")]
    public virtual ReactionType ReactionType { get; set; } = null!;
}
