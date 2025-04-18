// ------------------------------------------------------
// <copyright file="ApiKeyAuthenticationExtensions.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Api.Core.AuthenticationSchemes;
using DonkeyWork.Chat.Api.Core.Handlers;
using Microsoft.AspNetCore.Authentication;

namespace DonkeyWork.Chat.Api.Core.Extensions;

/// <summary>
/// Extension methods for API key authentication.
/// </summary>
public static class ApiKeyAuthenticationExtensions
{
    /// <summary>
    /// Adds API key authentication to the authentication builder.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <returns>An authentication builder.</returns>
    public static AuthenticationBuilder AddApiKeyAuthentication(this AuthenticationBuilder builder)
    {
        return builder.AddApiKeyAuthentication(ApiKeyAuthenticationOptions.DefaultScheme);
    }

    /// <summary>
    /// Adds API key authentication to the authentication builder with a custom scheme name.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="scheme">The custom scheme name.</param>
    /// <returns>An authentication builder.</returns>
    public static AuthenticationBuilder AddApiKeyAuthentication(this AuthenticationBuilder builder, string scheme)
    {
        return builder.AddApiKeyAuthentication(scheme, options => { });
    }

    /// <summary>
    /// Adds API key authentication to the authentication builder with a custom scheme name and configuration.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="scheme">The custom scheme name.</param>
    /// <param name="configureOptions">The configuration action.</param>
    /// <returns>An authentication builder.</returns>
    public static AuthenticationBuilder AddApiKeyAuthentication(
        this AuthenticationBuilder builder,
        string scheme,
        Action<ApiKeyAuthenticationOptions> configureOptions)
    {
        return builder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
            scheme,
            configureOptions);
    }
}