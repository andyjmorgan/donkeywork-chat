// ------------------------------------------------------
// <copyright file="OpenAiConfiguration.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace DonkeyWork.Chat.AiServices.Clients.OpenAi.Configuration;

/// <summary>
/// The open Ai configuration.
/// </summary>
public record OpenAiConfiguration
{
    /// <summary>
    /// Gets the api key.
    /// </summary>
    [Required]
    public string? ApiKey { get; init; } = string.Empty;
}