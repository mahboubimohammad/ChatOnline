using System;
using System.Collections.Generic;
using ChatOnlineApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatOnlineApi.Infrastructure.Persistence.Contexts;

public partial class ChatOnlineDbContext : DbContext
{
    public ChatOnlineDbContext(DbContextOptions<ChatOnlineDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Conversation> Conversations { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<MessageAttachment> MessageAttachments { get; set; }

    public virtual DbSet<MessageType> MessageTypes { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<ReactionType> ReactionTypes { get; set; }

    public virtual DbSet<SupportAgent> SupportAgents { get; set; }

    public virtual DbSet<UserPresence> UserPresences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Conversation_IsActive");

            entity.HasOne(d => d.SupportAgent).WithMany(p => p.Conversations).HasConstraintName("FK_Conversations_SupportAgents");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF_Messages_CreatedAt");

            entity.HasOne(d => d.Conversation).WithMany(p => p.Messages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Conversation");

            entity.HasOne(d => d.MessageReaction).WithMany(p => p.Messages).HasConstraintName("FK_Messages_ReactionTypes");

            entity.HasOne(d => d.MessageType).WithMany(p => p.Messages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_MessageType");

            entity.HasOne(d => d.ReplyToMessage).WithMany(p => p.InverseReplyToMessage).HasConstraintName("FK_Messages_ReplyToMessageId");
        });

        modelBuilder.Entity<MessageAttachment>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF_MessageAttachments_CreatedAt");

            entity.HasOne(d => d.Message).WithMany(p => p.MessageAttachments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MessageAttachments_Messages");
        });

        modelBuilder.Entity<MessageType>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF_Notifications_CreatedAt");

            entity.HasOne(d => d.Conversation).WithMany(p => p.Notifications).HasConstraintName("FK_Notifications_Conversations");

            entity.HasOne(d => d.Message).WithMany(p => p.Notifications).HasConstraintName("FK_Notifications_Messages");
        });

        modelBuilder.Entity<SupportAgent>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF_SupportAgents_CreatedAt");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_SupportAgents_IsActive");
        });

        modelBuilder.Entity<UserPresence>(entity =>
        {
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())", "DF_UserPresence_UpdatedAt");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
