// ------------------------------------------------------
// <copyright file="SerpApiConfiguration.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.SerpApi.Configuration;

/// <summary>
/// The serp api configuration.
/// </summary>
public record SerpApiConfiguration
{
    /// <summary>
    /// Gets the api key.
    /// </summary>
    [Required]
    [MinLength(1)]
    public string ApiKey { get; init; } = string.Empty;
}