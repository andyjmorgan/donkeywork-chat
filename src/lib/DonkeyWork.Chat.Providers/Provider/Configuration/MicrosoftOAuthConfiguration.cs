// ------------------------------------------------------
// <copyright file="MicrosoftOAuthConfiguration.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace DonkeyWork.Chat.Providers.Provider.Configuration;

/// <summary>
/// The microsoft OAuth configuration.
/// </summary>
public record MicrosoftOAuthConfiguration : BaseOAuthConfiguration
{
    /// <summary>
    /// Gets the tenant id.
    /// </summary>
    public string TenantId { get; init; } = "common";

    /// <summary>
    /// Gets the client id.
    /// </summary>
    [Required]
    public string ClientId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the client secret.
    /// </summary>
    public string ClientSecret { get; init; } = string.Empty;

    /// <summary>
    /// Gets the desired scopes.
    /// </summary>
    public string[] Scopes { get; init; } = [];
}