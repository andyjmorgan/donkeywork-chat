// ------------------------------------------------------
// <copyright file="ApiKeySummaryModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Api.Models.ApiKey;

/// <summary>
/// A summary of an API key.
/// </summary>
public record ApiKeySummaryModel
{
    /// <summary>
    /// Gets the id of the api key.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the created date of the API key.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

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
    /// Gets a hashed partial value of the api key.
    /// </summary>
    public string ApiKey { get; init; } = string.Empty;
}