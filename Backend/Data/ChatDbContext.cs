using System;
using System.Collections.Generic;
using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public partial class ChatDbContext : DbContext
{
    public ChatDbContext()
    {
    }

    public ChatDbContext(DbContextOptions<ChatDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attachment> Attachment { get; set; }

    public virtual DbSet<Chatroom> Chatroom { get; set; }

    public virtual DbSet<Chatroommember> Chatroommember { get; set; }

    public virtual DbSet<Chatuser> Chatuser { get; set; }

    public virtual DbSet<Message> Message { get; set; }

    public virtual DbSet<MsgAppartient> MsgAppartient { get; set; }

    public virtual DbSet<UserRecive> UserRecive { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone" })
            .HasPostgresEnum("auth", "oauth_authorization_status", new[] { "pending", "approved", "denied", "expired" })
            .HasPostgresEnum("auth", "oauth_client_type", new[] { "public", "confidential" })
            .HasPostgresEnum("auth", "oauth_registration_type", new[] { "dynamic", "manual" })
            .HasPostgresEnum("auth", "oauth_response_type", new[] { "code" })
            .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in" })
            .HasPostgresEnum("storage", "buckettype", new[] { "STANDARD", "ANALYTICS", "VECTOR" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("graphql", "pg_graphql")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(e => e.Idattachment).HasName("attachment_pkey");
        });

        modelBuilder.Entity<Chatroom>(entity =>
        {
            entity.HasKey(e => e.Idchatroom).HasName("chatroom_pkey");

            entity.Property(e => e.Roomcreatedat).HasDefaultValueSql("now()");

            entity.HasOne(d => d.IduserNavigation).WithMany(p => p.Chatroom).HasConstraintName("fk_chatroom_user_crea_user");
        });

        modelBuilder.Entity<Chatroommember>(entity =>
        {
            entity.HasKey(e => new { e.Idchatroom, e.Iduser }).HasName("pk_chatroommember");

            entity.Property(e => e.Useraddat).HasDefaultValueSql("now()");

            entity.HasOne(d => d.IdchatroomNavigation).WithMany(p => p.Chatroommember).HasConstraintName("fk_chatroom_chatroomm_chatroom");

            entity.HasOne(d => d.IduserNavigation).WithMany(p => p.Chatroommember).HasConstraintName("fk_chatroom_chatroomm_user");
        });

        modelBuilder.Entity<Chatuser>(entity =>
        {
            entity.HasKey(e => e.Iduser).HasName("USER_pkey");

            entity.Property(e => e.Iduser).HasDefaultValueSql("nextval('\"USER_iduser_seq\"'::regclass)");
            entity.Property(e => e.Createdat).HasDefaultValueSql("now()");
            entity.Property(e => e.Isconnect).HasDefaultValue(false);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Idmsg).HasName("message_pkey");

            entity.Property(e => e.Createdat).HasDefaultValueSql("now()");

            entity.HasOne(d => d.IduserNavigation).WithMany(p => p.Message).HasConstraintName("fk_message_user_send_user");

            entity.HasMany(d => d.Idattachment).WithMany(p => p.Idmsg)
                .UsingEntity<Dictionary<string, object>>(
                    "MsgContient",
                    r => r.HasOne<Attachment>().WithMany()
                        .HasForeignKey("Idattachment")
                        .HasConstraintName("fk_msg_cont_msg_conti_attachme"),
                    l => l.HasOne<Message>().WithMany()
                        .HasForeignKey("Idmsg")
                        .HasConstraintName("fk_msg_cont_msg_conti_message"),
                    j =>
                    {
                        j.HasKey("Idmsg", "Idattachment").HasName("pk_msg_contient");
                        j.ToTable("msg_contient");
                        j.IndexerProperty<long>("Idmsg").HasColumnName("idmsg");
                        j.IndexerProperty<long>("Idattachment").HasColumnName("idattachment");
                    });
        });

        modelBuilder.Entity<MsgAppartient>(entity =>
        {
            entity.HasKey(e => new { e.Idmsg, e.Idchatroom }).HasName("pk_msg_appartient");

            entity.Property(e => e.Msgsendchatroomat).HasDefaultValueSql("now()");

            entity.HasOne(d => d.IdchatroomNavigation).WithMany(p => p.MsgAppartient).HasConstraintName("fk_msg_appa_msg_appar_chatroom");

            entity.HasOne(d => d.IdmsgNavigation).WithMany(p => p.MsgAppartient).HasConstraintName("fk_msg_appa_msg_appar_message");
        });

        modelBuilder.Entity<UserRecive>(entity =>
        {
            entity.HasKey(e => new { e.Iduser, e.Idmsg }).HasName("pk_user_recive");

            entity.Property(e => e.Msgvue).HasDefaultValue(false);
            entity.Property(e => e.Recivedat).HasDefaultValueSql("now()");

            entity.HasOne(d => d.IdmsgNavigation).WithMany(p => p.UserRecive).HasConstraintName("fk_user_rec_user_reci_message");

            entity.HasOne(d => d.IduserNavigation).WithMany(p => p.UserRecive).HasConstraintName("fk_user_rec_user_reci_user");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
