// ------------------------------------------------------
// <copyright file="UserController.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Api.Core.Attributes;
using DonkeyWork.Chat.Api.Models;
using DonkeyWork.Chat.Api.Services.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace DonkeyWork.Chat.Api.Controllers;

/// <summary>
/// Handles authentication operations with Keycloak.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IAuthenticationService authService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class.
    /// </summary>
    /// <param name="authService">The authentication service.</param>
    public UserController(
        IAuthenticationService authService)
    {
        this.authService = authService;
    }

    /// <summary>
    /// Gets the current user info.
    /// </summary>
    /// <returns>User information if authenticated.</returns>
    [CookieOrApiKeyAuth]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserInformationResponse))]
    public IActionResult GetUserInformationAsync()
    {
        return this.Ok(this.authService.GetUserInfo(this.HttpContext));
    }
}