// ------------------------------------------------------
// <copyright file="IIntegrationRepository.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Providers;
using DonkeyWork.Chat.Persistence.Repository.Integration.Models;

namespace DonkeyWork.Chat.Persistence.Repository.Integration;

/// <summary>
/// An interface for the integration repository.
/// </summary>
public interface IIntegrationRepository
{
    /// <summary>
    /// Gets the user tokens.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A dictionary of the user tokens.</returns>
    public Task<List<UserIntegrationItem>> GetUserIntegrationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the user tokens.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A dictionary of the user tokens.</returns>
    public Task<List<UserOAuthTokenItem>> GetUserOAuthTokensAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the user tokens.
    /// </summary>
    /// <param name="providerType">The provider type.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A dictionary of the user tokens.</returns>
    public Task<UserOAuthTokenItem?> GetUserOAuthTokenAsync(UserProviderType providerType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the user tokens for the given provider type.
    /// </summary>
    /// <param name="type">The provider type.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A dictionary of the user tokens.</returns>
    public Task DeleteIntegrationAsync(UserProviderType type, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds the user OAuth token.
    /// </summary>
    /// <param name="oauthToken">The oauth token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task AddOrUpdateUserOAuthTokenAsync(UserOAuthTokenItem oauthToken, CancellationToken cancellationToken = default);
}