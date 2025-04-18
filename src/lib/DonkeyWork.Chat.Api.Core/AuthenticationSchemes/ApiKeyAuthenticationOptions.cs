// ------------------------------------------------------
// <copyright file="ApiKeyAuthenticationOptions.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using Microsoft.AspNetCore.Authentication;

namespace DonkeyWork.Chat.Api.Core.AuthenticationSchemes;

/// <summary>
/// Options for API key authentication.
/// </summary>
public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// Gets or sets the authentication scheme name.
    /// </summary>
    public const string DefaultScheme = "ApiKey";

    /// <summary>
    /// Gets or sets the display name for the authentication handler.
    /// </summary>
    public string DisplayName { get; set; } = "API Key";
}