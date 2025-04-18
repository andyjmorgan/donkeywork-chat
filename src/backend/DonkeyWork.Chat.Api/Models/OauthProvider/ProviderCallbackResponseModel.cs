// ------------------------------------------------------
// <copyright file="ProviderCallbackResponseModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Providers;

namespace DonkeyWork.Chat.Api.Models.Provider;

/// <summary>
/// Response model for provider callbacks.
/// </summary>
public class ProviderCallbackResponseModel
{
    /// <summary>
    /// Gets or sets the provider type.
    /// </summary>
    public UserProviderType ProviderType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the connection was successful.
    /// </summary>
    public bool Connected { get; set; }

    /// <summary>
    /// Gets or sets the granted scopes.
    /// </summary>
    public string[] Scopes { get; set; } = Array.Empty<string>();
}