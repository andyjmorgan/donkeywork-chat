// ------------------------------------------------------
// <copyright file="ProviderUrlResponseModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Providers;

namespace DonkeyWork.Chat.Api.Models.Provider;

/// <summary>
/// A model to get the provider url.
/// </summary>
public record ProviderUrlResponseModel
{
    /// <summary>
    /// Gets the provider type.
    /// </summary>
    public UserProviderType ProviderType { get; init; }

    /// <summary>
    /// Gets the authorization url.
    /// </summary>
    public string AuthorizationUrl { get; init; } = string.Empty;
}