using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models.Entities;

[Table("tokens")]
[Index("Iduser", Name = "idx_tokens_user")]
[Index("Token", Name = "tokens_token_key", IsUnique = true)]
public partial class Tokens
{
    [Key]
    [Column("idtoken")]
    public long Idtoken { get; set; }

    [Column("iduser")]
    public long Iduser { get; set; }

    [Column("token")]
    [StringLength(1000)]
    public string Token { get; set; } = null!;

    [Column("expiresat")]
    public DateTime Expiresat { get; set; }

    [Column("createdat")]
    public DateTime? Createdat { get; set; }

    [Column("isvalid")]
    public bool? Isvalid { get; set; }

    [ForeignKey("Iduser")]
    [InverseProperty("Tokens")]
    public virtual Chatuser IduserNavigation { get; set; } = null!;
}
