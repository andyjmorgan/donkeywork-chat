// ------------------------------------------------------
// <copyright file="ApiKeyModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Api.Models.ApiKey;

/// <summary>
/// An API key model.
/// </summary>
public record ApiKeyModel
{
    /// <summary>
    /// Gets the id of the API key.
    /// </summary>
    public Guid Id { get; init; }

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

    /// <summary>
    /// Gets the api key value.
    /// </summary>
    public string ApiKey { get; init; } = string.Empty;
}