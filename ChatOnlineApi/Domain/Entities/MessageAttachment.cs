using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChatOnlineApi.Domain.Entities;

[Index("MessageId", Name = "IX_MessageAttachments_MessageId")]
public partial class MessageAttachment
{
    [Key]
    public int Id { get; set; }

    public int MessageId { get; set; }

    [StringLength(255)]
    public string FileName { get; set; } = null!;

    [StringLength(500)]
    public string FileUrl { get; set; } = null!;

    public long FileSize { get; set; }

    [StringLength(150)]
    public string ContentType { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("MessageId")]
    [InverseProperty("MessageAttachments")]
    public virtual Message Message { get; set; } = null!;
}
