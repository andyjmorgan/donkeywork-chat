// ------------------------------------------------------
// <copyright file="GenericProviderController.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Api.Models;
using DonkeyWork.Chat.Common.Providers.GenericProvider;
using DonkeyWork.Chat.Common.Providers.GenericProvider.Implementations;
using DonkeyWork.Chat.Persistence.Repository.Integration;
using DonkeyWork.Chat.Persistence.Repository.Integration.Models;
using Microsoft.AspNetCore.Mvc;

namespace DonkeyWork.Chat.Api.Controllers;

/// <summary>
/// The generic provider controller.
/// </summary>
[ApiController]
[Route("api/{controller}")]
public class GenericProviderController(IIntegrationRepository integrationRepository)
    : ControllerBase
{
    /// <summary>
    /// Gets a configuration for a provider type.
    /// </summary>
    /// <param name="providerType">The provider type.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpGet("{providerType}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenericProviderConfigurationModel))]
    public async Task<IActionResult> GetGenericProviderConfiguration(GenericProviderType providerType)
    {
        var configurationType = BaseGenericProviderConfiguration.GetConfigurationByProviderType(providerType);
        var existingIntegration = await integrationRepository.GetGenericIntegrationAsync(providerType, this.HttpContext.RequestAborted);

        if (existingIntegration == null || string.IsNullOrWhiteSpace(existingIntegration?.Configuration))
        {
            return this.Ok(new SwarmpitConfiguration().ToGenericProviderConfiguration());
        }

        var config = BaseGenericProviderConfiguration.FromJson(existingIntegration.Configuration);
        if (config == null)
        {
            return this.Ok(configurationType.ToGenericProviderConfiguration());
        }

        return this.Ok(config.ToGenericProviderConfiguration());
    }

    /// <summary>
    /// Gets all known provider definitions.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpGet]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(GenericProvidersModel))]
    public async Task<IActionResult> GetGenericProvidersAsync()
    {
        var existingIntegrations = await integrationRepository.GetGenericIntegrationsAsync(this.HttpContext.RequestAborted);
        return this.Ok(new GenericProvidersModel()
        {
            Providers = new List<GenericProviderDefinition>()
            {
                new GenericProviderDefinition()
                {
                    Type = GenericProviderType.Swarmpit,
                    Description = "An open-source self-hosted Docker Swarm management UI.",
                    Name = "Swarmpit",
                    IsConnected = existingIntegrations.Any(x => x.ProviderType == GenericProviderType.Swarmpit),
                    IsEnabled = existingIntegrations.FirstOrDefault(x => x.ProviderType == GenericProviderType.Swarmpit)?.IsEnabled ?? false,
                    Image = "https://images.opencollective.com/swarmpit/81bdfdc/logo/256.png",
                    Tags = new List<string>()
                    {
                        "docker",
                        "swarmpit",
                        "self-hosted",
                    },
                    Capabilities = [],
                },
            },
        });
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

        await integrationRepository.UpsertGenericIntegrationAsync(new GenericIntegrationItem()
        {
            ProviderType = configurationModel.ProviderType,
            Configuration = configuration.ToJson(),
            IsEnabled = configurationModel.IsEnabled,
        });
        return this.Ok();
    }

    /// <summary>
    /// Deletes a configuration for a provider type.
    /// </summary>
    /// <param name="providerType">THe provider type.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpDelete("{providerType}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteGenericProviderConfiguration(GenericProviderType providerType)
    {
        await integrationRepository.DeleteGenericIntegrationAsync(providerType, this.HttpContext.RequestAborted);
        return this.NoContent();
    }
}