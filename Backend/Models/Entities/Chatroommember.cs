using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models.Entities;

[PrimaryKey("Idchatroom", "Iduser")]
[Table("chatroommember")]
[Index("Iduser", Name = "idx_chatroommember_user")]
public partial class Chatroommember
{
    [Key]
    [Column("idchatroom")]
    public long Idchatroom { get; set; }

    [Key]
    [Column("iduser")]
    public long Iduser { get; set; }

    [Column("useraddat")]
    public DateTime? Useraddat { get; set; }

    [ForeignKey("Idchatroom")]
    [InverseProperty("Chatroommember")]
    public virtual Chatroom IdchatroomNavigation { get; set; } = null!;

    [ForeignKey("Iduser")]
    [InverseProperty("Chatroommember")]
    public virtual Chatuser IduserNavigation { get; set; } = null!;
}
