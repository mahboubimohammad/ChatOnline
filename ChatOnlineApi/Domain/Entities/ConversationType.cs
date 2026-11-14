using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChatOnlineApi.Domain.Entities;

public partial class ConversationType
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Title { get; set; } = null!;
}
