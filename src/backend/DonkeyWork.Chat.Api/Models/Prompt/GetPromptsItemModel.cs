// ------------------------------------------------------
// <copyright file="GetPromptsItemModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Api.Models.Prompt;

/// <summary>
/// A prompt item.
/// </summary>
public record GetPromptsItemModel
{
    /// <summary>
    /// Gets the id.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the title.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Gets the description.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets the content.
    /// </summary>
    public string Content { get; init; } = string.Empty;

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