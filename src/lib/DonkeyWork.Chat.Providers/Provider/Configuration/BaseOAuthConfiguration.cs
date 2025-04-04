// ------------------------------------------------------
// <copyright file="BaseOAuthConfiguration.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace DonkeyWork.Chat.Providers.Provider.Configuration;

/// <summary>
/// A base class for OAuth configuration.
/// </summary>
public abstract record BaseOAuthConfiguration
{
    /// <summary>
    /// Gets the Authorization Url.
    /// </summary>
    [Required]
    [Url]
    public string AuthorizeUrl { get; init; } = string.Empty;

    /// <summary>
    /// Gets the token Url.
    /// </summary>
    [Required]
    [Url]
    public string TokenUrl { get; init; } = string.Empty;

    /// <summary>
    /// Gets the user info url.
    /// </summary>
    [Required]
    [Url]
    public string UserInfoUrl { get; init; } = string.Empty;
}