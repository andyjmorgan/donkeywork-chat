// ------------------------------------------------------
// <copyright file="IAuthenticationService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Api.Models;
using DonkeyWork.Chat.Api.Services.Keycloak;
using Microsoft.AspNetCore.Mvc;

namespace DonkeyWork.Chat.Api.Services.Authentication;

/// <summary>
/// Service for handling authentication operations.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Processes an authentication code exchange request.
    /// </summary>
    /// <param name="request">The token exchange request.</param>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>Result of the authentication operation.</returns>
    Task<IActionResult> ExchangeCodeAsync(TokenExchangeRequest request, HttpContext httpContext);

    /// <summary>
    /// Processes a logout request.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <param name="frontendRedirectUrl">Optional URL to redirect after logout. If provided, returns a redirect to Keycloak logout.</param>
    /// <returns>Result of the logout operation.</returns>
    Task<IActionResult> LogoutAsync(HttpContext httpContext, string? frontendRedirectUrl = null);

    /// <summary>
    /// Gets user information for the authenticated user.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>User information.</returns>
    GetUserInformationResponse GetUserInfo(HttpContext httpContext);
}