using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models.Entities;

[Table("chatroom")]
public partial class Chatroom
{
    [Key]
    [Column("idchatroom")]
    public long Idchatroom { get; set; }

    [Column("iduser")]
    public long Iduser { get; set; }

    [Column("roomcreatedat")]
    public DateTime? Roomcreatedat { get; set; }

    [InverseProperty("IdchatroomNavigation")]
    public virtual ICollection<Chatroommember> Chatroommember { get; set; } = new List<Chatroommember>();

    [ForeignKey("Iduser")]
    [InverseProperty("Chatroom")]
    public virtual Chatuser IduserNavigation { get; set; } = null!;

    [InverseProperty("IdchatroomNavigation")]
    public virtual ICollection<MsgAppartient> MsgAppartient { get; set; } = new List<MsgAppartient>();
}
