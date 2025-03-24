// ------------------------------------------------------
// <copyright file="AllowedModelsConfiguration.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace DonkeyWork.Chat.Api.Configuration;

/// <summary>
/// A configuration for allowed models.
/// </summary>
public record AllowedModelsConfiguration
{
    /// <summary>
    /// Gets a dictionary of allowed models.
    /// </summary>
    [Required]
    public Dictionary<string, List<string>> AllowedModels { get; init; } = [];
}