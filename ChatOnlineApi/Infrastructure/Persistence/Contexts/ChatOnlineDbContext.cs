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

    public virtual DbSet<ConversationParticipant> ConversationParticipants { get; set; }

    public virtual DbSet<ConversationPin> ConversationPins { get; set; }

    public virtual DbSet<ConversationType> ConversationTypes { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<MessageAttachment> MessageAttachments { get; set; }


    public virtual DbSet<MessageReaction> MessageReactions { get; set; }

    public virtual DbSet<MessageReadReceipt> MessageReadReceipts { get; set; }

    public virtual DbSet<MessageType> MessageTypes { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<ParticipantRole> ParticipantRoles { get; set; }

    public virtual DbSet<ReactionType> ReactionTypes { get; set; }

    public virtual DbSet<SupportAgent> SupportAgents { get; set; }

    public virtual DbSet<UserPresence> UserPresences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Conversa__3214EC07581BBA63");

            entity.Property(e => e.ConversationTypeId).HasDefaultValue(1, "DF_Conversations_ConversationTypeId");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Conversation_IsActive");
        });

        modelBuilder.Entity<ConversationParticipant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Conversa__3214EC078B858120");

            entity.HasIndex(e => new { e.ConversationId, e.UserId }, "UX_ConversationParticipants_Active")
                .IsUnique()
                .HasFilter("([IsActive]=(1))");

            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_ConversationParticipants_IsActive");
            entity.Property(e => e.JoinedAt).HasDefaultValueSql("(getdate())", "DF_ConversationParticipants_JoinedAt");
            entity.Property(e => e.RoleId).HasDefaultValue(3, "DF_ConversationParticipants_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.ConversationParticipants)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConversationParticipants_ParticipantRoles");
        });

        modelBuilder.Entity<ConversationPin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Conversa__3214EC071D4311F0");

            entity.Property(e => e.PinnedAt).HasDefaultValueSql("(getdate())", "DF_ConversationPins_PinnedAt");
        });

        modelBuilder.Entity<ConversationType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Conversa__3214EC079EF8C153");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Messages__3214EC07428F8949");

            entity.ToTable(tb => tb.HasTrigger("TR_Messages_MessageStatusId"));

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF_Messages_CreatedAt");
            entity.Property(e => e.DeletedAt).HasDefaultValueSql("(getdate())", "DF_Messages_DeletedAt");
            entity.Property(e => e.EditedAt).HasDefaultValueSql("(getdate())", "DF_Messages_EditedAt");

            entity.HasOne(d => d.MessageType).WithMany(p => p.Messages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_MessageType");

            entity.HasOne(d => d.ReplyToMessage).WithMany(p => p.InverseReplyToMessage).HasConstraintName("FK_Messages_ReplyToMessageId");
        });

        modelBuilder.Entity<MessageAttachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MessageA__3214EC075162B1E1");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Message).WithMany(p => p.MessageAttachments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MessageAttachments_Messages");
        });


        modelBuilder.Entity<MessageReaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MessageR__3214EC0774B8B5C3");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF__MessageRe__Creat__395884C4");

            entity.HasOne(d => d.Message).WithMany(p => p.MessageReactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MessageReactions_MessageId");

            entity.HasOne(d => d.ReactionType).WithMany(p => p.MessageReactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MessageReactions_ReactionType");
        });

        modelBuilder.Entity<MessageReadReceipt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MessageR__3214EC074CF561E2");

            entity.HasOne(d => d.Message).WithMany(p => p.MessageReadReceipts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MessageReadReceipts_Messages");
        });

        modelBuilder.Entity<MessageType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MessageT__3214EC07D68112B7");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC0732ECF190");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF_Notifications_CreatedAt");

            entity.HasOne(d => d.Conversation).WithMany(p => p.Notifications).HasConstraintName("FK_Notifications_Conversations");

            entity.HasOne(d => d.Message).WithMany(p => p.Notifications).HasConstraintName("FK_Notifications_Messages");
        });

        modelBuilder.Entity<ParticipantRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Particip__3214EC07435E9663");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<ReactionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reaction__3214EC0735A9360D");
        });

        modelBuilder.Entity<SupportAgent>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF_SupportAgents_CreatedAt");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_SupportAgents_IsActive");
        });

        modelBuilder.Entity<UserPresence>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserPres__3214EC0704A937C0");

            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())", "DF_UserPresence_UpdatedAt");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
