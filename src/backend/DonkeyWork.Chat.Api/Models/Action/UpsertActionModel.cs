// ------------------------------------------------------
// <copyright file="UpsertActionModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Actions;
using DonkeyWork.Chat.Common.Models.Providers.Tools;

namespace DonkeyWork.Chat.Api.Models.Action;

/// <summary>
/// Model for creating or updating an action.
/// </summary>
public class UpsertActionModel
{
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
    /// Gets or sets the allowed tools.
    /// </summary>
    public List<ToolProviderApplicationType> AllowedTools { get; set; } = [];

    /// <summary>
    /// Gets or sets the action model configuration.
    /// </summary>
    public ModelConfiguration ModelConfiguration { get; set; } = new ();
}
