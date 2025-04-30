// ------------------------------------------------------
// <copyright file="ActionItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Actions;
using DonkeyWork.Chat.Common.Models.Providers.Tools;

namespace DonkeyWork.Persistence.Agent.Repository.Action.Models;

/// <summary>
/// Represents an action item.
/// </summary>
public class ActionItem
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the system prompt ids.
    /// </summary>
    public List<Guid> SystemPromptIds { get; set; } = [];

    /// <summary>
    /// Gets or sets the user prompt ids.
    /// </summary>
    public List<Guid> UserPromptIds { get; set; } = [];

    /// <summary>
    /// Gets or sets the action model configuration.
    /// </summary>
    public ActionModelConfiguration ActionModelConfiguration { get; set; } = new ();

    /// <summary>
    /// Gets or sets the allowed tools.
    /// </summary>
    public List<ToolProviderApplicationType> AllowedTools { get; set; } = [];

    /// <summary>
    /// Gets or sets the created at date.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the updated at date.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}
