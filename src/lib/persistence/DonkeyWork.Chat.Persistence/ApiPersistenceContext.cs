// ------------------------------------------------------
// <copyright file="ApiPersistenceContext.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.UserContext;
using DonkeyWork.Chat.Persistence.Entity.Base;
using DonkeyWork.Chat.Persistence.Entity.Conversation;
using DonkeyWork.Chat.Persistence.Entity.Prompt;
using Microsoft.EntityFrameworkCore;

namespace DonkeyWork.Chat.Persistence;

/// <summary>
/// The context for the api layer.
/// </summary>
public class ApiPersistenceContext(DbContextOptions<ApiPersistenceContext> options, IUserContextProvider userContextProvider)
    : DbContext(options)
{
    /// <summary>
    /// Gets or sets the conversations.
    /// </summary>
    public DbSet<ConversationEntity> Conversations { get; set; }

    /// <summary>
    /// Gets or sets the conversation messages.
    /// </summary>
    public DbSet<ConversationMessageEntity> ConversationMessages { get; set; }

    /// <summary>
    /// Gets or sets the prompts.
    /// </summary>
    public DbSet<PromptEntity> Prompts { get; set; }

    /// <summary>
    /// Gets or sets the tool calls.
    /// </summary>
    public DbSet<ToolCallEntity> ToolCalls { get; set; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConversationEntity>()
            .HasMany(c => c.MessageEntities)
            .WithOne(c => c.Conversation)
            .HasForeignKey(c => c.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ConversationEntity>()
            .HasMany(c => c.ToolCallEntities)
            .WithOne(t => t.Conversation)
            .HasForeignKey(t => t.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ConversationMessageEntity>()
            .HasIndex(c => c.ConversationId)
            .IsUnique(false);

        modelBuilder.Entity<ConversationMessageEntity>()
            .HasIndex(c => c.MessagePairId)
            .IsUnique(false);

        modelBuilder.Entity<ToolCallEntity>()
            .HasIndex(c => c.ConversationId)
            .IsUnique(false);

        modelBuilder.Entity<ToolCallEntity>()
            .HasIndex(c => c.MessagePairId)
            .IsUnique(false);

        // Add the base user filter.
        modelBuilder.Entity<BaseUserEntity>()
            .HasQueryFilter(x => x.UserId == userContextProvider.UserId);
    }
}