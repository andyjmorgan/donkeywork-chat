// ------------------------------------------------------
// <copyright file="TokenRefreshWorkerConfiguration.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Api.Workers;

/// <summary>
/// A configuration for the token refresh worker.
/// </summary>
public record TokenRefreshWorkerConfiguration
{
    /// <summary>
    /// Gets the interval at which tokens are checked.
    /// </summary>
    public TimeSpan RefreshInterval { get; init; } = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Gets the threshold for token expiration.
    /// Tokens that expire within this timespan will be refreshed.
    /// </summary>
    public TimeSpan RefreshThreshold { get; init; } = TimeSpan.FromHours(2);

    /// <summary>
    /// Gets the maximum number of tokens to refresh in a single batch.
    /// </summary>
    public int BatchSize { get; init; } = 100;
}