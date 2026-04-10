using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models.Entities;

[Table("attachment")]
public partial class Attachment
{
    [Key]
    [Column("idattachment")]
    public long Idattachment { get; set; }

    [Column("attachmentsize")]
    [StringLength(30)]
    public string? Attachmentsize { get; set; }

    [Column("attachmentpath")]
    [StringLength(150)]
    public string Attachmentpath { get; set; } = null!;

    [Column("createdat")]
    public DateTime? Createdat { get; set; }

    [ForeignKey("Idattachment")]
    [InverseProperty("Idattachment")]
    public virtual ICollection<Message> Idmsg { get; set; } = new List<Message>();
}
