// ------------------------------------------------------
// <copyright file="AgentPersistenceContext.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;
using DonkeyWork.Chat.Common.Models.Prompt;
using DonkeyWork.Chat.Common.Services.UserContext;
using DonkeyWork.Persistence.Agent.Entity.Action;
using DonkeyWork.Persistence.Agent.Entity.ActionExecution;
using DonkeyWork.Persistence.Agent.Entity.Prompt;
using DonkeyWork.Persistence.Common.Entity.Base;
using Microsoft.EntityFrameworkCore;

namespace DonkeyWork.Persistence.Agent;

/// <summary>
/// A class representing the persistence context for agents.
/// </summary>
/// <param name="options">The options.</param>
/// <param name="userContextProvider">The provider.</param>
public class AgentPersistenceContext(DbContextOptions<AgentPersistenceContext> options, IUserContextProvider userContextProvider)
    : DbContext(options)
{
    /// <summary>
    /// Gets or sets the system prompts.
    /// </summary>
    public DbSet<SystemPromptEntity> SystemPrompts { get; set; }

    /// <summary>
    /// Gets or sets the prompts.
    /// </summary>
    public DbSet<ActionPromptEntity> ActionPrompts { get; set; }

    /// <summary>
    /// Gets or sets the actions.
    /// </summary>
    public DbSet<ActionEntity> Actions { get; set; }

    /// <summary>
    /// Gets or sets the action executions.
    /// </summary>
    public DbSet<ActionExecutionEntity> ActionExecutions { get; set; }

    /// <summary>
    /// Gets or sets the action system prompt relations.
    /// </summary>
    public DbSet<ActionSystemPromptRelationEntity> ActionSystemPromptRelations { get; set; }

    /// <summary>
    /// Gets or sets the action prompt relations.
    /// </summary>
    public DbSet<ActionPromptRelationEntity> ActionPromptRelations { get; set; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(nameof(AgentPersistenceContext));
        modelBuilder.Entity<BaseUserEntity>()
            .UseTpcMappingStrategy();

        modelBuilder.Entity<ActionEntity>()
            .HasIndex(a => new { a.Name, a.UserId })
            .IsUnique(true);

        modelBuilder.Entity<SystemPromptEntity>()
            .HasIndex(p => new { p.Name, p.UserId })
            .IsUnique(true);

        modelBuilder.Entity<ActionPromptEntity>()
            .HasIndex(p => new { p.Name, p.UserId })
            .IsUnique(true);

        modelBuilder.Entity<ActionSystemPromptRelationEntity>()
            .HasOne(a => a.Action)
            .WithMany(a => a.SystemPrompts)
            .HasForeignKey(a => a.ActionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ActionSystemPromptRelationEntity>()
            .HasOne(a => a.SystemPrompt)
            .WithMany()
            .HasForeignKey(a => a.SystemPromptId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ActionPromptRelationEntity>()
            .HasOne(a => a.Action)
            .WithMany(a => a.ActionPrompts)
            .HasForeignKey(a => a.ActionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ActionPromptRelationEntity>()
            .HasOne(a => a.ActionPrompt)
            .WithMany()
            .HasForeignKey(a => a.ActionPromptId)
            .OnDelete(DeleteBehavior.Restrict);

        var options = new JsonSerializerOptions
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            AllowOutOfOrderMetadataProperties = true,
        };

        modelBuilder.Entity<ActionPromptEntity>()
            .Property(e => e.Messages)
            .HasConversion(
                v => JsonSerializer.Serialize(v, options),
                v => JsonSerializer.Deserialize<List<PromptMessage>>(v, options) !);

        modelBuilder.Entity<BaseUserEntity>()
            .HasQueryFilter(x => x.UserId == userContextProvider.UserId);
    }
}
