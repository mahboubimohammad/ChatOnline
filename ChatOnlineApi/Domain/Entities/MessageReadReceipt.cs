using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChatOnlineApi.Domain.Entities;

[Index("MessageId", "UserId", Name = "UX_MessageReadReceipts_MessageId_UserId", IsUnique = true)]
public partial class MessageReadReceipt
{
    [Key]
    public int Id { get; set; }

    public int MessageId { get; set; }

    public int UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DeliveredAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ReadAt { get; set; }

    [ForeignKey("MessageId")]
    [InverseProperty("MessageReadReceipts")]
    public virtual Message Message { get; set; } = null!;
}
