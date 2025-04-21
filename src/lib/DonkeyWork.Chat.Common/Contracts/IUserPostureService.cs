// ------------------------------------------------------
// <copyright file="IUserPostureService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Providers;
using DonkeyWork.Chat.Common.Models.Providers.GenericProvider;

namespace DonkeyWork.Chat.Common.Contracts;

/// <summary>
/// An interface for a user posture service.
/// </summary>
public interface IUserPostureService
{
    /// <summary>
    /// Gets the user posture for the given provider.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<ToolProviderPosture> GetUserPosturesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the user posture for the given provider.
    /// </summary>
    /// <param name="providerType">The provider type.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<UserProviderPosture?> GetUserPostureAsync(
        UserProviderType providerType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the generic user posture for the given provider.
    /// </summary>
    /// <param name="providerType">The provider type.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<GenericProviderPosture?> GetUserGenericPostureAsync(
        GenericProviderType providerType,
        CancellationToken cancellationToken = default);
}