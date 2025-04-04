// ------------------------------------------------------
// <copyright file="TokenRefreshWorkerConfiguration.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Providers.Workers;

/// <summary>
/// A configuration for the token refresh worker.
/// </summary>
public record TokenRefreshWorkerConfiguration
{
    /// <summary>
    /// Gets or sets the interval at which tokens are checked.
    /// </summary>
    public TimeSpan RefreshInterval { get; init; } = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Gets or sets the threshold for token expiration.
    /// Tokens that expire within this timespan will be refreshed.
    /// </summary>
    public TimeSpan RefreshThreshold { get; init; } = TimeSpan.FromHours(1);

    /// <summary>
    /// Gets or sets the maximum number of tokens to refresh in a single batch.
    /// </summary>
    public int BatchSize { get; init; } = 100;
}