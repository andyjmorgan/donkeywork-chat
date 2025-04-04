// ------------------------------------------------------
// <copyright file="IOAuthProvider.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Providers.Models;

namespace DonkeyWork.Chat.Providers.Provider;

/// <summary>
/// An interface for an oauth provider.
/// </summary>
public interface IOAuthProvider
{
    /// <summary>
    /// Gets the authorization url for the provider.
    /// </summary>
    /// <param name="redirectUri">The redirect url.</param>
    /// <param name="state">The desired state.</param>
    /// <returns>An authorization url.</returns>
    Task<string> GetAuthorizationUrl(string redirectUri, string? state = null);

    /// <summary>
    /// Exchanges the code for a token.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <param name="redirectUri">The redirect Url.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<OAuthTokenResult> ExchangeCodeForToken(string code, string redirectUri);
}