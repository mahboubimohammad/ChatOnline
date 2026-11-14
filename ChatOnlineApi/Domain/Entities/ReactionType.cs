using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChatOnlineApi.Domain.Entities;

[Index("Title", Name = "UQ_ReactionTypes_Title", IsUnique = true)]
public partial class ReactionType
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Title { get; set; } = null!;

    [InverseProperty("ReactionType")]
    public virtual ICollection<MessageReaction> MessageReactions { get; set; } = new List<MessageReaction>();
}
