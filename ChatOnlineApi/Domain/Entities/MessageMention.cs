using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChatOnlineApi.Domain.Entities;

[Index("MessageId", "UserId", Name = "UX_MessageMentions_MessageId_UserId", IsUnique = true)]
public partial class MessageMention
{
    [Key]
    public int Id { get; set; }

    public int MessageId { get; set; }

    public int UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }
}
