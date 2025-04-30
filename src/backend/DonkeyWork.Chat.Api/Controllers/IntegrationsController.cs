// ------------------------------------------------------
// <copyright file="IntegrationsController.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.AiTooling.Services;
using DonkeyWork.Chat.Api.Models.OauthProvider;
using DonkeyWork.Chat.Api.Models.Tool;
using DonkeyWork.Chat.Common.Contracts;
using DonkeyWork.Chat.Common.Models.Providers.Tools;
using DonkeyWork.Chat.Common.Models.Providers.Tools.GenericProvider;
using DonkeyWork.Chat.Providers.Provider;
using DonkeyWork.Persistence.User.Repository.Integration;
using DonkeyWork.Persistence.User.Repository.Integration.Models;
using Microsoft.AspNetCore.Mvc;

namespace DonkeyWork.Chat.Api.Controllers;

/// <summary>
/// A controller for managing tools.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class IntegrationsController : ControllerBase
{
    private readonly IToolService toolService;
    private readonly IUserPostureService userPostureService;
    private readonly IIntegrationRepository integrationRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationsController"/> class.
    /// </summary>
    /// <param name="toolService">The tool service.</param>
    /// <param name="userPostureService">The user posture service.</param>
    /// <param name="integrationRepository">The integration repository.</param>
    public IntegrationsController(IToolService toolService, IUserPostureService userPostureService, IIntegrationRepository integrationRepository)
    {
        this.toolService = toolService;
        this.userPostureService = userPostureService;
        this.integrationRepository = integrationRepository;
    }

    /// <summary>
    /// Gets the providers.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="providerType">The provider type.</param>
    /// <param name="redirectUrl">The redirectUrl.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpGet("authorizeUrl/{ProviderType}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProviderUrlResponseModel))]
    public async Task<IActionResult> GetProviders(
        [FromServices] IServiceProvider serviceProvider,
        [FromRoute] ToolProviderType providerType,
        [FromQuery] string? redirectUrl = null)
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
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="providerType">The provider type.</param>
    /// <param name="code">The authorization code.</param>
    /// <param name="error">Any error that occurred.</param>
    /// <param name="redirectUrl">The redirect URL that was used.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpGet("callback/{providerType}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProviderResponseModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> HandleCallbackAsync(
        [FromServices] IServiceProvider serviceProvider,
        [FromRoute] ToolProviderType providerType,
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
            await this.integrationRepository.AddOrUpdateUserOAuthTokenAsync(
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
    /// Gets a configuration for a provider type.
    /// </summary>
    /// <param name="providerType">The provider type.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpGet("{providerType}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenericProviderConfigurationModel))]
    public async Task<IActionResult> GetGenericProviderConfiguration(ToolProviderType providerType)
    {
        var configurationType = BaseGenericProviderConfiguration.GetConfigurationByProviderType(providerType);
        var existingIntegration = await this.integrationRepository.GetGenericIntegrationAsync(providerType, this.HttpContext.RequestAborted);

        if (existingIntegration == null || string.IsNullOrWhiteSpace(existingIntegration?.Configuration))
        {
            return this.Ok(configurationType.ToGenericProviderConfiguration());
        }

        var config = BaseGenericProviderConfiguration.FromJson(existingIntegration.Configuration);
        if (config == null)
        {
            return this.Ok(configurationType.ToGenericProviderConfiguration());
        }

        return this.Ok(config.ToGenericProviderConfiguration());
    }

    /// <summary>
    /// Creates or updates a configuration for a provider type.
    /// </summary>
    /// <param name="configurationModel">The configuration model.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> UpsertGenericProviderConfiguration(GenericProviderConfigurationModel configurationModel)
    {
        var configuration = BaseGenericProviderConfiguration
            .GetConfigurationByProviderType(configurationModel.ProviderType)
            .FromGenericProviderConfiguration(configurationModel);

        await this.integrationRepository.UpsertGenericIntegrationAsync(new GenericIntegrationItem()
        {
            ProviderType = configurationModel.ProviderType,
            Configuration = configuration.ToJson(),
            IsEnabled = configurationModel.IsEnabled,
        });
        return this.Ok();
    }

    /// <summary>
    /// Gets the tools the user has access to.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetToolProviders()
    {
        var toolProviders = ToolProviderDefinition.GetToolProviders();
        var userPosture = await this.userPostureService.GetUserPosturesAsync(this.HttpContext.RequestAborted);

        GetToolProvidersModel response = new GetToolProvidersModel();
        foreach (var toolProvider in toolProviders)
        {
            response.ToolProviders.Add(
                new ToolProvidersModel()
                {
                    ProviderType = toolProvider.Value.ProviderType,
                    Applications = toolProvider.Value.Applications,
                    Description = toolProvider.Value.Description,
                    Icon = toolProvider.Value.Icon,
                    Name = toolProvider.Value.Name,
                    AuthorizationType = toolProvider.Value.AuthorizationType,
                    IsConnected = toolProvider.Value.AuthorizationType == ToolProviderAuthorizationType.OAuth ?
                        userPosture.UserTokens.Any(x => x.ProviderType == toolProvider.Value.ProviderType)
                        : userPosture.GenericIntegrations.Any(x => x.ProviderType == toolProvider.Value.ProviderType),
                });
        }

        return this.Ok(response);
    }

    /// <summary>
    /// Deletes a user provider.
    /// </summary>
    /// <param name="providerType">The provider type.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpDelete]
    [Route("{providerType}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteUserProviderAsync(
        [FromRoute] ToolProviderType providerType)
    {
        var toolProviders = ToolProviderDefinition.GetToolProviders();
        if (!toolProviders.TryGetValue(providerType, out var toolProvider))
        {
            return this.NotFound();
        }

        if (toolProvider.AuthorizationType == ToolProviderAuthorizationType.OAuth)
        {
            await this.integrationRepository.DeleteOauthIntegrationAsync(providerType, this.HttpContext.RequestAborted);
        }

        {
            await this.integrationRepository.DeleteGenericIntegrationAsync(providerType, this.HttpContext.RequestAborted);
        }

        return this.NoContent();
    }
}
