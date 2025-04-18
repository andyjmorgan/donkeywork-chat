// ------------------------------------------------------
// <copyright file="GenericIntegration.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Providers.GenericProvider;

namespace DonkeyWork.Chat.Persistence.Repository.Integration.Models;

/// <summary>
/// A record representing a generic integration.
/// </summary>
public record GenericIntegrationItem
{
    /// <summary>
    /// Gets the provider type.
    /// </summary>
    required public GenericProviderType ProviderType { get; init; }

    /// <summary>
    /// Gets a value indicating whether the integration is enabled.
    /// </summary>
    public bool IsEnabled { get; init; } = false;

    /// <summary>
    /// Gets the provider metadata.
    /// </summary>
    public string? Configuration { get; init; }
}