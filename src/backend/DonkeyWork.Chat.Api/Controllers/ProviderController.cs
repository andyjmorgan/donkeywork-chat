// ------------------------------------------------------
// <copyright file="ProviderController.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Api.Models.Provider;
using DonkeyWork.Chat.Common.Providers;
using DonkeyWork.Chat.Persistence.Repository.Integration;
using DonkeyWork.Chat.Persistence.Repository.Integration.Models;
using DonkeyWork.Chat.Providers.Provider;
using Microsoft.AspNetCore.Mvc;

namespace DonkeyWork.Chat.Api.Controllers;

/// <summary>
/// Handles provider operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProviderController(
    IServiceProvider serviceProvider,
    IIntegrationRepository integrationRepository)
    : ControllerBase
{
    /// <summary>
    /// Gets the users provider.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProviderResponseModel))]
    public async Task<IActionResult> GetUserProviderAsync()
    {
        var integrations = await integrationRepository.GetUserOAuthIntegrationsAsync();
        var response = new UserProviderResponseModel();
        foreach (var integration in integrations)
        {
           response.ProviderConfiguration[integration.Provider] = integration.Scopes;
        }

        return this.Ok(response);
    }

    /// <summary>
    /// Gets the providers.
    /// </summary>
    /// <param name="providerType">The provider type.</param>
    /// <param name="redirectUrl">The redirectUrl.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpGet("authorizeUrl/{ProviderType}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProviderUrlResponseModel))]
    public async Task<IActionResult> GetProviders([FromRoute] UserProviderType providerType, [FromQuery] string? redirectUrl = null)
    {
        var oAuthProvider = serviceProvider.GetRequiredKeyedService<IOAuthProvider>(providerType.ToString());
        var result = await oAuthProvider.GetAuthorizationUrl(redirectUrl ?? string.Empty, string.Empty);
        return this.Ok(new ProviderUrlResponseModel()
        {
            ProviderType = providerType,
            AuthorizationUrl = result,
        });
    }

    /// <summary>
    /// Handles the OAuth callback from providers.
    /// </summary>
    /// <param name="providerType">The provider type.</param>
    /// <param name="code">The authorization code.</param>
    /// <param name="error">Any error that occurred.</param>
    /// <param name="redirectUrl">The redirect URL that was used.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpGet("callback/{providerType}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProviderResponseModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> HandleCallbackAsync(
        [FromRoute] UserProviderType providerType,
        [FromQuery] string? code,
        [FromQuery] string? error,
        [FromQuery] string? redirectUrl = null)
    {
        // Handle any errors from the provider
        if (!string.IsNullOrEmpty(error))
        {
            return this.BadRequest(new ProblemDetails
            {
                Title = "OAuth Error",
                Detail = $"Provider returned error: {error}",
                Status = StatusCodes.Status400BadRequest,
            });
        }

        // Validate the authorization code
        if (string.IsNullOrEmpty(code))
        {
            return this.BadRequest(new ProblemDetails
            {
                Title = "Missing Authorization Code",
                Detail = "No authorization code was provided in the callback",
                Status = StatusCodes.Status400BadRequest,
            });
        }

        try
        {
            var oAuthProvider = serviceProvider.GetRequiredKeyedService<IOAuthProvider>(providerType.ToString());
            var tokenResult = await oAuthProvider.ExchangeCodeForToken(code, redirectUrl ?? string.Empty);
            await integrationRepository.AddOrUpdateUserOAuthTokenAsync(
                new UserOAuthTokenItem()
                {
                    Expiration = tokenResult.ExpiresOn,
                    Provider = providerType,
                    Scopes = tokenResult.Scopes.ToList(),
                    Metadata = new Dictionary<UserProviderDataKeyType, string>()
                    {
                        [UserProviderDataKeyType.AccessToken] = tokenResult.AccessToken,
                        [UserProviderDataKeyType.RefreshToken] = tokenResult.RefreshToken,
                    },
                },
                this.HttpContext.RequestAborted);

            return this.Ok(new ProviderCallbackResponseModel
            {
                ProviderType = providerType,
                Connected = true,
                Scopes = tokenResult.Scopes,
            });
        }
        catch (Exception ex)
        {
            return this.BadRequest(new ProblemDetails
            {
                Title = "OAuth Exchange Failed",
                Detail = $"Failed to exchange code for token: {ex.Message}",
                Status = StatusCodes.Status400BadRequest,
            });
        }
    }

    /// <summary>
    /// Deletes a user provider.
    /// </summary>
    /// <param name="providerType">The provider type.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpDelete]
    [Route("{providerType}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteUserProviderAsync([FromRoute] UserProviderType providerType)
    {
        await integrationRepository.DeleteOauthIntegrationAsync(providerType);
        return this.NoContent();
    }
}