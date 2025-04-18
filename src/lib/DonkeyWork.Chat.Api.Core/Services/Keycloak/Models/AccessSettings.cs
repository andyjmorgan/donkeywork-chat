// ------------------------------------------------------
// <copyright file="AccessSettings.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Serialization;

namespace DonkeyWork.Chat.Api.Core.Services.Keycloak.Models;

/// <summary>
/// Represents access settings for a Keycloak user.
/// </summary>
public class AccessSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether the user can manage group membership.
    /// </summary>
    [JsonPropertyName("manageGroupMembership")]
    public bool ManageGroupMembership { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user has view access.
    /// </summary>
    [JsonPropertyName("view")]
    public bool View { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user can map roles.
    /// </summary>
    [JsonPropertyName("mapRoles")]
    public bool MapRoles { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user can impersonate others.
    /// </summary>
    [JsonPropertyName("impersonate")]
    public bool Impersonate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user has management access.
    /// </summary>
    [JsonPropertyName("manage")]
    public bool Manage { get; set; }
}