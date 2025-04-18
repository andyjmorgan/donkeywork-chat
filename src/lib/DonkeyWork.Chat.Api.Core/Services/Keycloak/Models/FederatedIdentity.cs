// ------------------------------------------------------
// <copyright file="FederatedIdentity.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Serialization;

namespace DonkeyWork.Chat.Api.Core.Services.Keycloak.Models;

/// <summary>
/// Represents a federated identity linked to a Keycloak user.
/// </summary>
public class FederatedIdentity
{
    /// <summary>
    /// Gets or sets the identity provider name.
    /// </summary>
    [JsonPropertyName("identityProvider")]
    public string IdentityProvider { get; set; } = default!;

    /// <summary>
    /// Gets or sets the user ID in the identity provider's system.
    /// </summary>
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the username in the identity provider's system.
    /// </summary>
    [JsonPropertyName("userName")]
    public string UserName { get; set; } = default!;
}