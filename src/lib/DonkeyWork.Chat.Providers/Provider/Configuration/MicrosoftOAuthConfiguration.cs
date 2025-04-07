// ------------------------------------------------------
// <copyright file="MicrosoftOAuthConfiguration.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Providers.Provider.Configuration;

/// <summary>
/// The microsoft OAuth configuration.
/// </summary>
public record MicrosoftOAuthConfiguration : BaseOAuthConfiguration
{
    /// <summary>
    /// Gets the tenant id.
    /// </summary>
    public string TenantId { get; init; } = "common";
}