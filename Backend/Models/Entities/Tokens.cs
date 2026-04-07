using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models.Entities;

[Index("ExpiresAt", Name = "idx_tokens_expiresat")]
[Index("IsValid", Name = "idx_tokens_isvalid")]
[Index("Token", Name = "idx_tokens_token")]
public partial class Tokens
{
    [Key]
    public long Id { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsValid { get; set; }
}
