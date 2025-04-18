// ------------------------------------------------------
// <copyright file="ApiKeyInfoItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Persistence.Repository.ApiKey.Models;

/// <summary>
/// An API key item.
/// </summary>
public record ApiKeyInfoItem : ApiKeyItem
{
    /// <summary>
    /// Gets the id of the API key.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the created time.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }
}