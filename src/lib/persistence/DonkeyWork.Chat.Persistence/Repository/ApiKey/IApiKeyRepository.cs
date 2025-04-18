// ------------------------------------------------------
// <copyright file="IApiKeyRepository.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Persistence.Common;
using DonkeyWork.Chat.Persistence.Repository.ApiKey.Models;

namespace DonkeyWork.Chat.Persistence.Repository.ApiKey;

/// <summary>
/// An interface for the API key repository.
/// </summary>
public interface IApiKeyRepository
{
    /// <summary>
    /// Creates a new API key for the specified user.
    /// </summary>
    /// <param name="apiKeyItem">The api key model.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="ApiKeyItem"/>.</returns>
    public Task<ApiKeyItem> CreateApiKeyAsync(ApiKeyItem apiKeyItem, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an API key for the specified user.
    /// </summary>
    /// <param name="id">The api key id.</param>
    /// <param name="apiKey">The updated model.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="ApiKeyItem"/>.</returns>
    public Task<ApiKeyItem?> UpdateApiKeyAsync(Guid id, ApiKeyItem apiKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all API keys for the specified user.
    /// </summary>
    /// <param name="pagingParameters">The paging parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<GetApiKeysResponseItem> GetApiKeysAsync(PagingParameters pagingParameters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific API key for the specified user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<ApiKeyItem?> GetApiKeyAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an api key.
    /// </summary>
    /// <param name="id">The api key id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task DeleteApiKeyAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the user id for the specified API key.
    /// </summary>
    /// <param name="apiKey">The api key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<Guid?> GetUserIdByApiKeyAsync(string apiKey, CancellationToken cancellationToken = default);
}