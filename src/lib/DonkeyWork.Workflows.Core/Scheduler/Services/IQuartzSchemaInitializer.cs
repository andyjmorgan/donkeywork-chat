// ------------------------------------------------------
// <copyright file="IQuartzSchemaInitializer.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Workflows.Core.Scheduler.Services;

/// <summary>
/// A service for initializing the Quartz schema.
/// </summary>
public interface IQuartzSchemaInitializer
{
    /// <summary>
    /// Ensures that the Quartz schema is created in the database.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task EnsureSchemaCreatedAsync();
}
