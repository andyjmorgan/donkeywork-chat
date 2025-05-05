// ------------------------------------------------------
// <copyright file="AgentModelNodeMetadata.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Actions;
using DonkeyWork.Chat.Common.Models.Providers.Tools;

namespace DonkeyWork.Chat.Common.Models.Agents.Metadata;

/// <summary>
/// Gets or sets the agent model node metadata.
/// </summary>
public class AgentModelNodeMetadata : AgentNodeBaseMetadata
{
    /// <summary>
    /// Gets or sets a value indicating whether the model uses dynamic tooling.
    /// </summary>
    public bool IsDynamicTooling { get; set; } = false;

    /// <summary>
    /// Gets or sets the allowed tools.
    /// </summary>
    public List<ToolProviderApplicationType> AllowedTools { get; set; } = [];

    /// <summary>
    /// Gets or sets the system prompts.
    /// </summary>
    public List<Guid> SystemPrompts { get; set; } = [];

    /// <summary>
    /// Gets or sets the action model configuration.
    /// </summary>
    public ModelConfiguration ModelConfiguration { get; set; } = new ModelConfiguration();
}
