// ------------------------------------------------------
// <copyright file="PromptContentItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Persistence.Agent.Repository.Prompt.Models.SystemPrompt;

/// <summary>
/// The contents of a prompt.
/// </summary>
public record PromptContentItem
{
    /// <summary>
    /// Gets the prompt name.
    /// </summary>
    required public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the prompt description.
    /// </summary>
    required public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets the prompt content.
    /// </summary>
    public List<string> Content { get; init; } = [];
}
