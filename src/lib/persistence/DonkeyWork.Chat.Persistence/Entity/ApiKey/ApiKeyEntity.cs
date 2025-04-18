// ------------------------------------------------------
// <copyright file="ApiKeyEntity.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using DonkeyWork.Chat.Persistence.Entity.Base;

namespace DonkeyWork.Chat.Persistence.Entity.ApiKey;

/// <summary>
/// Gets or sets the API key entity.
/// </summary>
public class ApiKeyEntity : BaseUserEntity
{
    /// <summary>
    /// Gets or sets the API key.
    /// </summary>
    [MaxLength(256)]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the API key is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the description of the API key.
    /// </summary>
    [MaxLength(1024)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets the name of the API key.
    /// </summary>
    [MaxLength(128)]
    public string Name { get; set; } = string.Empty;
}