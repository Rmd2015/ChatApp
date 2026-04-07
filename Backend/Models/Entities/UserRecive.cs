using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models.Entities;

[PrimaryKey("Iduser", "Idmsg")]
[Table("user_recive")]
[Index("Idmsg", Name = "idx_user_recive_msg")]
public partial class UserRecive
{
    [Key]
    [Column("iduser")]
    public long Iduser { get; set; }

    [Key]
    [Column("idmsg")]
    public long Idmsg { get; set; }

    [Column("recivedat")]
    public DateTime? Recivedat { get; set; }

    [Column("msgvue")]
    public bool? Msgvue { get; set; }

    [Column("vueat")]
    public DateTime? Vueat { get; set; }

    [ForeignKey("Idmsg")]
    [InverseProperty("UserRecive")]
    public virtual Message IdmsgNavigation { get; set; } = null!;

    [ForeignKey("Iduser")]
    [InverseProperty("UserRecive")]
    public virtual Chatuser IduserNavigation { get; set; } = null!;
}
