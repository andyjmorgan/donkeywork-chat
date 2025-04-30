// ------------------------------------------------------
// <copyright file="SerpApiConfiguration.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DonkeyWork.Chat.Common.Models.Providers.Tools.GenericProvider.Implementations;

/// <summary>
/// The serp api configuration.
/// </summary>
public class SerpApiConfiguration : BaseGenericProviderConfiguration
{
    /// <summary>
    /// Gets the api key.
    /// </summary>
    [Description("Api Key")]
    [PasswordPropertyText]
    [Required]
    public string ApiKey { get; init; } = string.Empty;

    /// <inheritdoc />
    public override GenericProviderConfigurationModel ToGenericProviderConfiguration()
    {
        return this.CreateGenericProviderConfiguration(ToolProviderType.Serp);
    }

    /// <inheritdoc />
    public override BaseGenericProviderConfiguration FromGenericProviderConfiguration(GenericProviderConfigurationModel configurationModel)
    {
        return new SerpApiConfiguration()
        {
            Enabled = configurationModel.IsEnabled,
            ApiKey = configurationModel.Properties[nameof(this.ApiKey)].Value?.ToString() ?? string.Empty,
        };
    }
}