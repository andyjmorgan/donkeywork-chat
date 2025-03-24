// ------------------------------------------------------
// <copyright file="AuthController.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Api.Services.Keycloak;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IAuthenticationService = DonkeyWork.Chat.Api.Services.Authentication.IAuthenticationService;

namespace DonkeyWork.Chat.Api.Controllers;

/// <summary>
/// Handles authentication operations with Keycloak.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService authService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="authService">The authentication service.</param>
    public AuthController(
        IAuthenticationService authService)
    {
        this.authService = authService;
    }

    /// <summary>
    /// Exchange an authorization code for tokens and establish a session.
    /// </summary>
    /// <param name="request">The token exchange request.</param>
    /// <returns>A success or error result.</returns>
    [HttpPost("exchange")]
    [AllowAnonymous]
    public async Task<IActionResult> ExchangeCode([FromBody] TokenExchangeRequest request)
    {
        return await this.authService.ExchangeCodeAsync(request, this.HttpContext);
    }

    // Removed RefreshTokens endpoint - using standard cookie-based auth that doesn't require manual refresh

    /// <summary>
    /// Logs the user out.
    /// </summary>
    /// <returns>A success result.</returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        return await this.authService.LogoutAsync(this.HttpContext);
    }
}