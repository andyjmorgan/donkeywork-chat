// ------------------------------------------------------
// <copyright file="GetApiKeysModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Api.Models.ApiKey;

/// <summary>
/// A model for api keys.
/// </summary>
public record GetApiKeysModel
{
    /// <summary>
    /// Gets the count of api keys.
    /// </summary>
    public int Count { get; init; }

    /// <summary>
    /// Gets the api keys.
    /// </summary>
    public IEnumerable<ApiKeySummaryModel> ApiKeys { get; init; } = [];
}