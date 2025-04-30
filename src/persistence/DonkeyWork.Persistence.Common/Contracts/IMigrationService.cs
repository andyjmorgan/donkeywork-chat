// ------------------------------------------------------
// <copyright file="IMigrationService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Persistence.Common.Contracts;

/// <summary>
/// A service for migrations.
/// </summary>
public interface IMigrationService
{
    /// <summary>
    /// Performs pending migrations.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task MigrateAsync();
}