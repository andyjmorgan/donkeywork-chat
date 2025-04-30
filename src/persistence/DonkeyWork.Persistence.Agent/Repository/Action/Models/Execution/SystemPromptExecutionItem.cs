// ------------------------------------------------------
// <copyright file="SystemPromptExecutionItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Persistence.Agent.Repository.Action.Models.Execution;

/// <summary>
/// A system prompt execution item.
/// </summary>
public sealed class SystemPromptExecutionItem
{
    /// <summary>
    /// Gets or sets the prompt id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the prompt name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets the prompt content.
    /// </summary>
    public List<string> Content { get; init; } = [];
}
