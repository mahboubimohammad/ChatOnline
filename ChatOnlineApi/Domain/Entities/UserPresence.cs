using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChatOnlineApi.Domain.Entities;

[Table("UserPresence")]
[Index("UserId", Name = "UX_UserPresence_UserId", IsUnique = true)]
public partial class UserPresence
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public bool IsOnline { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastSeenAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; }
}
