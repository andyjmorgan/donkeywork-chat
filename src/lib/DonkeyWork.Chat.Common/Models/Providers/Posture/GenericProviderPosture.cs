// ------------------------------------------------------
// <copyright file="GenericProviderPosture.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Providers.Tools;
using DonkeyWork.Chat.Common.Models.Providers.Tools.GenericProvider;

namespace DonkeyWork.Chat.Common.Models.Providers.Posture;

/// <summary>
/// A user provider posture.
/// </summary>
public record GenericProviderPosture
{
    /// <summary>
    /// Gets the provider type.
    /// </summary>
    public ToolProviderType ProviderType { get; init; }

    /// <summary>
    /// Gets the provider configuration.
    /// </summary>
    required public BaseGenericProviderConfiguration Configuration { get; init; }
}