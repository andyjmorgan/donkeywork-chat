// ------------------------------------------------------
// <copyright file="GetActionPromptsItemModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Prompt;

namespace DonkeyWork.Chat.Api.Models.Prompt;

/// <summary>
/// An action prompt item.
/// </summary>
public record GetActionPromptsItemModel
{
    /// <summary>
    /// Gets the id.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the Name.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the description.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets the prompt variables.
    /// </summary>
    public Dictionary<string, PromptVariable> Variables { get; init; } = [];

    /// <summary>
    /// Gets the prompt messages.
    /// </summary>
    public List<PromptMessage> Messages { get; init; } = [];

    /// <summary>
    /// Gets the usage count.
    /// </summary>
    public int UsageCount { get; init; }

    /// <summary>
    /// Gets the Created At.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Gets the Updated At.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; init; }
}