// ------------------------------------------------------
// <copyright file="GeminiConfiguration.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace DonkeyWork.Chat.AiServices.Clients.Google.Configuration;

/// <summary>
/// Google configuration.
/// </summary>
public record GeminiConfiguration
{
    /// <summary>
    /// Gets the api key.
    /// </summary>
    [Required]
    public string ApiKey { get; init; } = string.Empty;
}