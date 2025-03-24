// ------------------------------------------------------
// <copyright file="AnthropicConfiguration.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace DonkeyWork.Chat.AiServices.Clients.Anthropic.Configuration;

/// <summary>
/// Anthropic configuration.
/// </summary>
public record AnthropicConfiguration
{
    /// <summary>
    /// Gets the api key.
    /// </summary>
    [Required]
    public string ApiKey { get; init; } = string.Empty;
}