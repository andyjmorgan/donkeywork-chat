// ------------------------------------------------------
// <copyright file="GenericProviderEntity.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using DonkeyWork.Chat.Common.Providers.GenericProvider;
using DonkeyWork.Chat.Persistence.Entity.Base;

namespace DonkeyWork.Chat.Persistence.Entity.Provider;

/// <summary>
/// Gets or sets the provider type.
/// </summary>
public class GenericProviderEntity : BaseUserEntity
{
    /// <summary>
    /// Gets or sets the provider type.
    /// </summary>
    public GenericProviderType ProviderType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the provider is enabled.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the token metadata.
    /// </summary>
    [MaxLength(10240)]
    public string? Configuration { get; set; }
}