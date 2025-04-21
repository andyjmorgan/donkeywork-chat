// ------------------------------------------------------
// <copyright file="ApiPersistenceContext.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Commonn.Services.UserContext;
using DonkeyWork.Chat.Persistence.Entity.ApiKey;
using DonkeyWork.Chat.Persistence.Entity.Conversation;
using DonkeyWork.Chat.Persistence.Entity.Prompt;
using DonkeyWork.Chat.Persistence.Entity.Provider;
using DonkeyWork.Persistence.Common.Entity.Base;
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
    /// Gets or sets the system prompts.
    /// </summary>
    public DbSet<SystemPromptEntity> SystemPrompts { get; set; }

    /// <summary>
    /// Gets or sets the prompts.
    /// </summary>
    public DbSet<GenericPromptEntity> Prompts { get; set; }

    /// <summary>
    /// Gets or sets the tool calls.
    /// </summary>
    public DbSet<ToolCallEntity> ToolCalls { get; set; }

    /// <summary>
    /// Gets or sets the provider tokens.
    /// </summary>
    public DbSet<UserTokenEntity> UserTokens { get; set; }

    /// <summary>
    /// Gets or sets the generic provider tokens.
    /// </summary>
    public DbSet<GenericProviderEntity> GenericProviders { get; set; }

    /// <summary>
    /// Gets or sets the Api keys.
    /// </summary>
    public DbSet<ApiKeyEntity> ApiKeys { get; set; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ApiPersistence");

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

        modelBuilder.Entity<BaseUserEntity>()
            .UseTpcMappingStrategy();

        modelBuilder.Entity<ApiKeyEntity>()
            .HasIndex(x => x.ApiKey)
            .IsUnique(true);

        modelBuilder.Entity<SystemPromptEntity>()
            .HasIndex(p => new { p.Title, p.UserId })
            .IsUnique(true);

        modelBuilder.Entity<GenericProviderEntity>()
            .HasIndex(p => new { p.ProviderType, p.UserId })
            .IsUnique(true);

        modelBuilder.Entity<GenericProviderEntity>()
            .HasIndex(p => new { p.ProviderType, p.UserId })
            .IsUnique(true);

        // Add the base user filter.
        modelBuilder.Entity<BaseUserEntity>()
            .HasQueryFilter(x => x.UserId == userContextProvider.UserId);
    }
}