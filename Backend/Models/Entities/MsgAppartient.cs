using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models.Entities;

[PrimaryKey("Idmsg", "Idchatroom")]
[Table("msg_appartient")]
[Index("Idchatroom", Name = "idx_message_chatroom")]
public partial class MsgAppartient
{
    [Key]
    [Column("idmsg")]
    public long Idmsg { get; set; }

    [Key]
    [Column("idchatroom")]
    public long Idchatroom { get; set; }

    [Column("msgsendchatroomat")]
    public DateTime? Msgsendchatroomat { get; set; }

    [ForeignKey("Idchatroom")]
    [InverseProperty("MsgAppartient")]
    public virtual Chatroom IdchatroomNavigation { get; set; } = null!;

    [ForeignKey("Idmsg")]
    [InverseProperty("MsgAppartient")]
    public virtual Message IdmsgNavigation { get; set; } = null!;
}
