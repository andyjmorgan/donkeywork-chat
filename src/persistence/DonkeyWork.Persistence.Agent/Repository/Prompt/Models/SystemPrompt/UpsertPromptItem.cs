// ------------------------------------------------------
// <copyright file="UpsertPromptItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Persistence.Agent.Repository.Prompt.Models.SystemPrompt;

/// <summary>
/// Represents an item to upsert a prompt.
/// </summary>
public record UpsertPromptItem
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