using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models.Entities;

[Table("chatuser")]
[Index("Username", Name = "USER_username_key", IsUnique = true)]
public partial class Chatuser
{
    [Key]
    [Column("iduser")]
    public long Iduser { get; set; }

    [Column("username")]
    [StringLength(50)]
    public string Username { get; set; } = null!;

    [Column("password")]
    [StringLength(60)]
    public string Password { get; set; } = null!;

    [Column("isconnect")]
    public bool? Isconnect { get; set; }

    [Column("createdat")]
    public DateTime? Createdat { get; set; }

    [InverseProperty("IduserNavigation")]
    public virtual ICollection<Chatroom> Chatroom { get; set; } = new List<Chatroom>();

    [InverseProperty("IduserNavigation")]
    public virtual ICollection<Chatroommember> Chatroommember { get; set; } = new List<Chatroommember>();

    [InverseProperty("IduserNavigation")]
    public virtual ICollection<Message> Message { get; set; } = new List<Message>();

    [InverseProperty("IduserNavigation")]
    public virtual ICollection<UserRecive> UserRecive { get; set; } = new List<UserRecive>();
}
