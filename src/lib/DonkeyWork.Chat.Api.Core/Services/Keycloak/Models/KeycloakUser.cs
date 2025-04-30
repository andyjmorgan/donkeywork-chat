// ------------------------------------------------------
// <copyright file="KeycloakUser.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Serialization;

namespace DonkeyWork.Chat.Api.Core.Services.Keycloak.Models;

/// <summary>
/// Represents a user retrieved from Keycloak.
/// </summary>
public class KeycloakUser
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    /// <summary>
    /// Gets or sets the username of the user.
    /// </summary>
    [JsonPropertyName("username")]
    public string Username { get; set; } = default!;

    /// <summary>
    /// Gets or sets the first name of the user.
    /// </summary>
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = default!;

    /// <summary>
    /// Gets or sets the last name of the user.
    /// </summary>
    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = default!;

    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = default!;

    /// <summary>
    /// Gets or sets a value indicating whether the email has been verified.
    /// </summary>
    [JsonPropertyName("emailVerified")]
    public bool EmailVerified { get; set; }

    /// <summary>
    /// Gets or sets additional attributes for the user.
    /// </summary>
    [JsonPropertyName("attributes")]
    public Dictionary<string, List<string>> Attributes { get; set; } = [];

    /// <summary>
    /// Gets or sets the timestamp when the user was created.
    /// </summary>
    [JsonPropertyName("createdTimestamp")]
    public long CreatedTimestamp { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the account is enabled.
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether time-based one-time password (TOTP) is enabled.
    /// </summary>
    [JsonPropertyName("totp")]
    public bool Totp { get; set; }

    /// <summary>
    /// Gets or sets the credential types that can be disabled.
    /// </summary>
    [JsonPropertyName("disableableCredentialTypes")]
    public List<string> DisableableCredentialTypes { get; set; } = [];

    /// <summary>
    /// Gets or sets the required actions for the user.
    /// </summary>
    [JsonPropertyName("requiredActions")]
    public List<string> RequiredActions { get; set; } = new ();

    /// <summary>
    /// Gets or sets the federated identities associated with the user.
    /// </summary>
    [JsonPropertyName("federatedIdentities")]
    public List<FederatedIdentity> FederatedIdentities { get; set; } = [];

    /// <summary>
    /// Gets or sets the not before timestamp.
    /// </summary>
    [JsonPropertyName("notBefore")]
    public int NotBefore { get; set; }

    /// <summary>
    /// Gets or sets the access settings for the user.
    /// </summary>
    [JsonPropertyName("access")]
    public AccessSettings Access { get; set; } = new ();
}
