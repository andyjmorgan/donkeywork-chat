// ------------------------------------------------------
// <copyright file="UpsertActionPromptModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Prompt;

namespace DonkeyWork.Chat.Api.Models.Prompt;

/// <summary>
/// An action prompt add or update item.
/// </summary>
public class UpsertActionPromptModel
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
    /// Gets the prompt variables.
    /// </summary>
    public Dictionary<string, PromptVariable> Variables { get; init; } = [];

    /// <summary>
    /// Gets the prompt messages.
    /// </summary>
    public List<ActionPromptMessageModel> Messages { get; init; } = [];
}