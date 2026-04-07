using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models.Entities;

[Table("message")]
[Index("Iduser", Name = "idx_message_sender")]
public partial class Message
{
    [Key]
    [Column("idmsg")]
    public long Idmsg { get; set; }

    [Column("iduser")]
    public long Iduser { get; set; }

    [Column("content")]
    public string? Content { get; set; }

    [Column("createdat")]
    public DateTime? Createdat { get; set; }

    [ForeignKey("Iduser")]
    [InverseProperty("Message")]
    public virtual Chatuser IduserNavigation { get; set; } = null!;

    [InverseProperty("IdmsgNavigation")]
    public virtual ICollection<MsgAppartient> MsgAppartient { get; set; } = new List<MsgAppartient>();

    [InverseProperty("IdmsgNavigation")]
    public virtual ICollection<UserRecive> UserRecive { get; set; } = new List<UserRecive>();

    [ForeignKey("Idmsg")]
    [InverseProperty("Idmsg")]
    public virtual ICollection<Attachment> Idattachment { get; set; } = new List<Attachment>();
}
