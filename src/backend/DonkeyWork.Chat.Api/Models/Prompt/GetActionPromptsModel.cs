// ------------------------------------------------------
// <copyright file="GetActionPromptsModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Api.Models.Prompt;

/// <summary>
/// Gets the action prompts model.
/// </summary>
public record GetActionPromptsModel
{
    /// <summary>
    /// Gets the count of action prompts.
    /// </summary>
    public int Count { get; init; }

    /// <summary>
    /// Gets the action prompts.
    /// </summary>
    public IEnumerable<GetActionPromptsItemModel> Prompts { get; init; } = [];
}