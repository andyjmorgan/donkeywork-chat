// ------------------------------------------------------
// <copyright file="ApiKeyAuthenticationHandler.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Security.Claims;
using System.Text.Encodings.Web;
using DonkeyWork.Chat.Api.Core.AuthenticationSchemes;
using DonkeyWork.Chat.Api.Core.Services.Keycloak;
using DonkeyWork.Chat.Common.UserContext;
using DonkeyWork.Chat.Persistence.Repository.ApiKey;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DonkeyWork.Chat.Api.Core.Handlers;

/// <summary>
/// Authentication handler for API key authentication.
/// </summary>
public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private const string ApiKeyHeaderName = "X-Api-Key";
    private readonly IApiKeyRepository apiKeyRepository;
    private readonly IKeycloakClient keycloakClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyAuthenticationHandler"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="encoder">The encoder.</param>
    /// <param name="apiKeyRepository">The API key repository.</param>
    /// <param name="keycloakClient">The Keycloak client.</param>
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IApiKeyRepository apiKeyRepository,
        IKeycloakClient keycloakClient)
        : base(options, logger, encoder) // Removed the clock/timeProvider parameter
    {
        this.apiKeyRepository = apiKeyRepository;
        this.keycloakClient = keycloakClient;
    }

    /// <inheritdoc />
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (this.Context.User?.Identity?.IsAuthenticated == true)
        {
            return AuthenticateResult.NoResult();
        }

        if(!this.Request.Headers.Authorization.Contains("Bearer"))
        {
            this.Request.Headers[ApiKeyHeaderName] = this.Request.Headers.Authorization.FirstOrDefault()?.Split(' ')[1];
        }

        if (!this.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
        {
            return AuthenticateResult.NoResult();
        }

        var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(providedApiKey))
        {
            return AuthenticateResult.NoResult();
        }

        var userId = await this.apiKeyRepository.GetUserIdByApiKeyAsync(providedApiKey);
        if (!userId.HasValue)
        {
            return AuthenticateResult.Fail("Invalid API key");
        }

        var userInfo = await this.keycloakClient.GetUserInfoByIdAsync(userId.Value);
        if (userInfo == null)
        {
            return AuthenticateResult.Fail("Failed to retrieve user information");
        }

        // Create claims for the user
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, userInfo.Id),
            new Claim("EmailVerified", userInfo.EmailVerified.ToString()),
            new Claim(ClaimTypes.Name, userInfo.Username),
            new Claim(ClaimTypes.Email, userInfo.Email),
            new Claim(ClaimTypes.Sid, userInfo.Id.ToString()),
            new Claim(ClaimTypes.GivenName, userInfo.FirstName),
            new Claim(ClaimTypes.Surname, userInfo.LastName),
        };

        var idp = userInfo.Attributes.FirstOrDefault(c => c.Key == "IDP");
        if (idp.Value != null)
        {
            claims.Add(new Claim("IDP", idp.Value.FirstOrDefault() ?? string.Empty));
        }

        var identity = new ClaimsIdentity(claims, this.Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, this.Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}