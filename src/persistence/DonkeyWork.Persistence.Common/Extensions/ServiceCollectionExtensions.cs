// ------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Persistence.Common.Contracts;
using DonkeyWork.Persistence.Common.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DonkeyWork.Persistence.Common.Extensions;

/// <summary>
/// A migration service implementation.
/// </summary>
/// <typeparam name="TContext">The type of the context.</typeparam>
public class MigrationService<TContext> : IMigrationService
    where TContext : DbContext
{
    private readonly TContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="MigrationService{TContext}"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public MigrationService(TContext context)
    {
        this.context = context;
    }

    /// <inheritdoc />
    public async Task MigrateAsync()
    {
        await this.context.Database.MigrateAsync();
    }
}