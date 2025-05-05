// ------------------------------------------------------
// <copyright file="AgentStringFormatterNodeMetadata.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Agents.Metadata;

/// <summary>
/// Agents string formatter node metadata entity.
/// </summary>
public class AgentStringFormatterNodeMetadata : AgentNodeBaseMetadata
{
    /// <summary>
    /// Gets or sets the string template.
    /// </summary>
    required public string Template { get; set; } = string.Empty;
}
