// ------------------------------------------------------
// <copyright file="ToolDefinition.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Providers.Tools;

namespace DonkeyWork.Chat.AiTooling.Base.Models;

/// <summary>
/// Gets or sets a tool definition.
/// </summary>
public class ToolDefinition
{
    /// <summary>
    /// Gets or sets the provider.
    /// </summary>
    required public ToolProviderType Provider { get; set; }

    /// <summary>
    /// Gets or sets the provider application.
    /// </summary>
    required public ToolProviderApplicationType Application { get; set; }

    /// <summary>
    /// Gets or sets the tool name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tool description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tool parameters.
    /// </summary>
    public string Parameters { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tool function definition.
    /// </summary>
    public ToolFunctionDefinition ToolFunctionDefinition { get; set; } = new ToolFunctionDefinition();

    /// <summary>
    /// Gets or sets the Tool.
    /// </summary>
    public ITool? Tool { get; set; }
}