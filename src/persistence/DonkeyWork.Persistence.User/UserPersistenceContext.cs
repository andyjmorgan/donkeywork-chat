// ------------------------------------------------------
// <copyright file="UserPersistenceContext.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Services.UserContext;
using DonkeyWork.Persistence.Common.Entity.Base;
using DonkeyWork.Persistence.User.Entity.ApiKey;
using DonkeyWork.Persistence.User.Entity.Provider;
using Microsoft.EntityFrameworkCore;

namespace DonkeyWork.Persistence.User;

/// <summary>
/// The context for the api layer.
/// </summary>
public class UserPersistenceContext(DbContextOptions<UserPersistenceContext> options, IUserContextProvider userContextProvider)
    : DbContext(options)
{
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
        modelBuilder.HasDefaultSchema(nameof(UserPersistenceContext));

        modelBuilder.Entity<BaseUserEntity>()
            .UseTpcMappingStrategy();
        modelBuilder.Entity<BaseUserEntity>()
            .HasQueryFilter(x => x.UserId == userContextProvider.UserId);

        modelBuilder.Entity<ApiKeyEntity>()
            .HasIndex(x => x.ApiKey)
            .IsUnique();

        modelBuilder.Entity<GenericProviderEntity>()
            .HasIndex(p => new { p.ProviderType, p.UserId })
            .IsUnique();

        modelBuilder.Entity<GenericProviderEntity>()
            .HasIndex(p => new { p.ProviderType, p.UserId })
            .IsUnique();
    }
}
