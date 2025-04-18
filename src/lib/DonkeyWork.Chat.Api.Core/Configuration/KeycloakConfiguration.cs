// ------------------------------------------------------
// <copyright file="KeycloakConfiguration.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Api.Core.Configuration;

/// <summary>
/// The keycloak configuration.
/// </summary>
public record KeycloakConfiguration
{
    /// <summary>
    /// Gets the authentication url.
    /// </summary>
    public string AuthenticationUrl { get; init; } = string.Empty;

    /// <summary>
    /// Gets the realm.
    /// </summary>
    public string Realm { get; init; } = string.Empty;

    /// <summary>
    /// Gets the metadata address.
    /// </summary>
    public string MetadataAddress { get; init; } = string.Empty;

    /// <summary>
    /// Gets the issuer.
    /// </summary>
    public string ValidIssuer { get; init; } = string.Empty;

    /// <summary>
    /// Gets the back channel address..
    /// </summary>
    public string BackChannelAddress { get; init; } = string.Empty;

    /// <summary>
    /// Gets the back channel admin address.
    /// </summary>
    public string BackChannelAdminAddress { get; init; } = string.Empty;

    /// <summary>
    /// Gets the audience.
    /// </summary>
    public string Audience { get; init; } = string.Empty;

    /// <summary>
    /// Gets the client ID.
    /// </summary>
    public string ClientId { get; init; } = "donkeywork";

    /// <summary>
    /// Gets the client secret.
    /// </summary>
    public string ClientSecret { get; init; } = string.Empty;
}