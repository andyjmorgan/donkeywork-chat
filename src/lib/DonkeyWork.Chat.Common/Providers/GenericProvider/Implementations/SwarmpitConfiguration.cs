// ------------------------------------------------------
// <copyright file="SwarmpitConfiguration.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DonkeyWork.Chat.Common.Providers.GenericProvider.Implementations;

/// <summary>
/// A configuration class for Swarmpit.
/// </summary>
public class SwarmpitConfiguration : BaseGenericProviderConfiguration
{
    /// <summary>
    /// Gets or sets the base URL for the Swarmpit API.
    /// </summary>
    [Description("Host Address")]
    [Required]
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the API token for authentication.
    /// </summary>
    [Description("Api Key")]
    [PasswordPropertyText]
    [Required]
    public string ApiKey { get; set; } = string.Empty;

    /// <inheritdoc />
    public override GenericProviderConfigurationModel ToGenericProviderConfiguration()
    {
        return this.CreateGenericProviderConfiguration(GenericProviderType.Swarmpit);
    }

    /// <inheritdoc />
    public override BaseGenericProviderConfiguration FromGenericProviderConfiguration(GenericProviderConfigurationModel configurationModel)
    {
        return new SwarmpitConfiguration
        {
            Enabled = configurationModel.IsEnabled,
            ApiKey = configurationModel.Properties[nameof(this.ApiKey)].Value?.ToString() ?? string.Empty,
            BaseUrl = configurationModel.Properties[nameof(this.BaseUrl)].Value?.ToString() ?? string.Empty,
        };
    }
}