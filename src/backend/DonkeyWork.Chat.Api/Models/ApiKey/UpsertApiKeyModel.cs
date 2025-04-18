// ------------------------------------------------------
// <copyright file="UpsertApiKeyModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Api.Models.ApiKey;

/// <summary>
/// A model for creating an API key.
/// </summary>
public record UpsertApiKeyModel
{
    /// <summary>
    /// Gets the name of the API key.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the description of the API key.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets a value indicating whether the API key is enabled.
    /// </summary>
    public bool IsEnabled { get; init; }
}