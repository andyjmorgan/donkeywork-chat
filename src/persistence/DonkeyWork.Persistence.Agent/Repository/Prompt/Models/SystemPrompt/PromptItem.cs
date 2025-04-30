// ------------------------------------------------------
// <copyright file="PromptItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Persistence.Agent.Repository.Prompt.Models.SystemPrompt;

/// <summary>
/// A prompt item.
/// </summary>
public record PromptItem : BasePromptItem
{
    /// <summary>
    /// Gets the content.
    /// </summary>
    public List<string> Content { get; init; } = [];
}