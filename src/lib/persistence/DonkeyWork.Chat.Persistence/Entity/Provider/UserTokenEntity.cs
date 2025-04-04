// ------------------------------------------------------
// <copyright file="UserTokenEntity.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel.DataAnnotations.Schema;
using DonkeyWork.Chat.Common.Providers;
using DonkeyWork.Chat.Persistence.Entity.Base;

namespace DonkeyWork.Chat.Persistence.Entity.Provider;

/// <summary>
/// A user token entity.
/// </summary>
public class UserTokenEntity : BaseUserEntity
{
    /// <summary>
    /// Gets the provider type.
    /// </summary>
    public UserProviderType ProviderType { get; init; }

    /// <summary>
    /// Gets or sets the scopes of the token.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public List<string> Scopes { get; set; } = [];

    /// <summary>
    /// Gets or sets the token metadata.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public Dictionary<UserProviderDataKeyType, string> Data { get; set; } = [];

    /// <summary>
    /// Gets or sets the expiry of the token.
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }
}