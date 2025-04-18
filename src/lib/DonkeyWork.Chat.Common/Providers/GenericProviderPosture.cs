// ------------------------------------------------------
// <copyright file="GenericProviderPosture.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Providers.GenericProvider;

namespace DonkeyWork.Chat.Common.Providers;

/// <summary>
/// A user provider posture.
/// </summary>
public record GenericProviderPosture
{
    /// <summary>
    /// Gets the provider type.
    /// </summary>
    public GenericProviderType ProviderType { get; init; }

    /// <summary>
    /// Gets the provider configuration.
    /// </summary>
    required public BaseGenericProviderConfiguration Configuration { get; init; }
}