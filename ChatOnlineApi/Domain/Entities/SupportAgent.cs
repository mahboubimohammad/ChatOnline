using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChatOnlineApi.Domain.Entities;

[Index("UserId", Name = "UQ_SupportAgents_UserId", IsUnique = true)]
public partial class SupportAgent
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    [StringLength(200)]
    public string FullName { get; set; } = null!;

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }
}
