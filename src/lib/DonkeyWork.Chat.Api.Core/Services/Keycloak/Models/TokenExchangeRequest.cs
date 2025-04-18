// ------------------------------------------------------
// <copyright file="TokenExchangeRequest.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Api.Core.Services.Keycloak.Models;

/// <summary>
/// Model for the token exchange request.
/// </summary>
public class TokenExchangeRequest
{
    /// <summary>
    /// Gets or sets the authorization code.
    /// </summary>
    required public string Code { get; set; }

    /// <summary>
    /// Gets or sets the code verifier.
    /// </summary>
    required public string CodeVerifier { get; set; }

    /// <summary>
    /// Gets or sets the redirect URI.
    /// </summary>
    required public string RedirectUri { get; set; }
}