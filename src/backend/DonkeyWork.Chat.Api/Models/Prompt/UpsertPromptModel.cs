// ------------------------------------------------------
// <copyright file="UpsertPromptModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Api.Models.Prompt;

/// <summary>
/// A prompt add or update item.
/// </summary>
public class UpsertPromptModel
{
    /// <summary>
    /// Gets the title.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the description.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets the content.
    /// </summary>
    public List<string> Content { get; init; } = [];
}