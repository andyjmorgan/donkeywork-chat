// ------------------------------------------------------
// <copyright file="AuthenticationService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Security.Authentication;
using System.Security.Claims;
using System.Text.Json;
using DonkeyWork.Chat.Api.Core.Services.Keycloak;
using DonkeyWork.Chat.Api.Core.Services.Keycloak.Models;
using DonkeyWork.Chat.Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace DonkeyWork.Chat.Api.Services.Authentication;

/// <summary>
/// Implementation of the authentication service.
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IKeycloakClient keycloakClient;
    private readonly ILogger<AuthenticationService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
    /// </summary>
    /// <param name="keycloakClient">The Keycloak client.</param>
    /// <param name="logger">The logger.</param>
    public AuthenticationService(
        IKeycloakClient keycloakClient,
        ILogger<AuthenticationService> logger)
    {
        this.keycloakClient = keycloakClient;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IActionResult> ExchangeCodeAsync(TokenExchangeRequest request, HttpContext httpContext)
    {
        try
        {
            // Use the Keycloak client to exchange code for tokens
            var tokenResponse = await this.keycloakClient.ExchangeCodeForTokensAsync(
                request.Code,
                request.CodeVerifier,
                request.RedirectUri);

            if (tokenResponse == null)
            {
                return new StatusCodeResult(500);
            }

            var accessToken = tokenResponse.Value.GetProperty("access_token").GetString();
            var refreshToken = tokenResponse.Value.GetProperty("refresh_token").GetString();
            var expiresIn = tokenResponse.Value.GetProperty("expires_in").GetInt32();

            // Extract user claims
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(accessToken);

            var claims = new List<Claim>();

            // Add required claims for identity
            claims.Add(new Claim(ClaimTypes.NameIdentifier, jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? string.Empty));
            claims.Add(new Claim("EmailVerified", jwtToken.Claims.FirstOrDefault(c => c.Type == "email_verified")?.Value ?? string.Empty));
            claims.Add(new Claim(ClaimTypes.Name, jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? string.Empty));
            claims.Add(new Claim(ClaimTypes.Email, jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? string.Empty));
            claims.Add(new Claim(ClaimTypes.Sid, jwtToken.Claims.FirstOrDefault(c => c.Type == "sid")?.Value ?? string.Empty));
            claims.Add(new Claim("IDP", jwtToken.Claims.FirstOrDefault(c => c.Type == "IDP")?.Value ?? string.Empty));
            claims.Add(new Claim(ClaimTypes.GivenName, jwtToken.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value ?? string.Empty));
            claims.Add(new Claim(ClaimTypes.Surname, jwtToken.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value ?? string.Empty));

            // Add roles if present
            var roles = jwtToken.Claims.Where(c => c.Type == "realm_access/roles" || c.Type == "resource_access/roles");
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Value));
            }

            // Add session token storage
            var tokens = new SessionTokens
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn),
            };

            // Store tokens in the session
            httpContext.Session.SetString("UserTokens", JsonSerializer.Serialize(tokens));

            // Create identity and sign in
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(1),
                });

            return new OkObjectResult(new { success = true });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Token exchange failed");
            return new StatusCodeResult(500);
        }
    }

    /// <inheritdoc/>
    public async Task<IActionResult> LogoutAsync(HttpContext httpContext, string? frontendRedirectUrl = null)
    {
        try
        {
            // Get the refresh token from the session
            var tokensJson = httpContext.Session.GetString("UserTokens");
            if (!string.IsNullOrEmpty(tokensJson))
            {
                var tokens = JsonSerializer.Deserialize<SessionTokens>(tokensJson);
                if (tokens?.RefreshToken != null)
                {
                    // Invalidate the token at Keycloak using the Keycloak client
                    await this.keycloakClient.LogoutAsync(tokens.RefreshToken);
                }
            }

            // Clear the local session regardless of Keycloak response
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            httpContext.Session.Clear();

            // If a redirect URL is provided, return a redirect to Keycloak logout
            // This ensures the Keycloak session cookie is also cleared
            if (!string.IsNullOrEmpty(frontendRedirectUrl))
            {
                // Get the full Keycloak logout URL with the frontend redirect
                var keycloakLogoutUrl = this.keycloakClient.GetKeycloakLogoutUrl(frontendRedirectUrl);
                return new RedirectResult(keycloakLogoutUrl);
            }

            // Otherwise just return a success response with the Keycloak logout URL
            // Always use the current origin for the redirect - this should NOT be used as frontendRedirectUrl should be provided
            // This is just a fallback with a strong default that won't work properly in development
            var defaultRedirectUrl = "https://chat.donkeywork.dev/";

            // Frontend can use this URL to redirect the user
            return new OkObjectResult(new
            {
                success = true,
                keycloakLogoutUrl = this.keycloakClient.GetKeycloakLogoutUrl(defaultRedirectUrl),
            });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Logout failed");

            // Clear the local session even if Keycloak call fails
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            httpContext.Session.Clear();

            // Still provide the Keycloak logout URL even if there was an error
            // Use the same default redirect URL as the success case - frontendRedirectUrl should always be provided
            var defaultRedirectUrl = "https://chat.donkeywork.dev/";

            return new OkObjectResult(
                new
                {
                    success = true,
                    error = "Logout partially completed",
                    keycloakLogoutUrl = this.keycloakClient.GetKeycloakLogoutUrl(defaultRedirectUrl),
                });
        }
    }

    /// <inheritdoc/>
    public GetUserInformationResponse GetUserInfo(HttpContext httpContext)
    {
        var user = httpContext.User;

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            throw new InvalidCredentialException("The user is missing or the user is not authenticated.");
        }

        return new GetUserInformationResponse
        {
            Id = Guid.Parse(userId),
            UserName = user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
            EmailAddress = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
            FirstName = user.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty,
            FamilyName = user.FindFirst(ClaimTypes.Surname)?.Value ?? string.Empty,
        };
    }
}