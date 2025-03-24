// ------------------------------------------------------
// <copyright file="AllowedModelsResponse.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Api.Models.Models;

/// <summary>
/// Gets the allowed models response.
/// </summary>
public class AllowedModelsResponse
{
    /// <summary>
    /// Gets or sets a dictionary of allowed models.
    /// </summary>
    public Dictionary<string, List<string>> AllowedModels { get; set; } = [];

    /// <summary>
    /// Gets or sets the default model.
    /// </summary>
    public KeyValuePair<string, string> DefaultModel { get; set; }
}