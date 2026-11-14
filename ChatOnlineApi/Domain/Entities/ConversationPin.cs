using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChatOnlineApi.Domain.Entities;

[Index("ConversationId", "MessageId", Name = "UX_ConversationPins_ConversationId_MessageId", IsUnique = true)]
public partial class ConversationPin
{
    [Key]
    public int Id { get; set; }

    public int ConversationId { get; set; }

    public int MessageId { get; set; }

    public int PinnedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime PinnedAt { get; set; }
}
