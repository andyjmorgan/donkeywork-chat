// ------------------------------------------------------
// <copyright file="ToolProviderApplicationDefinition.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Providers.Tools;

/// <summary>
/// A tool provider application definition.
/// </summary>
public class ToolProviderApplicationDefinition
{
    /// <summary>
    /// Gets or sets the provider type.
    /// </summary>
    public ToolProviderType Provider { get; set; }

    /// <summary>
    /// Gets or sets the Tool provider application type.
    /// </summary>
    public ToolProviderApplicationType Application { get; set; }

    /// <summary>
    /// Gets or sets the Application name.
    /// </summary>
    required public string Name { get; set; }

    /// <summary>
    /// Gets or sets the Application description.
    /// </summary>
    required public string Description { get; set; }

    /// <summary>
    /// Gets or sets the Application icon.
    /// </summary>
    required public string Icon { get; set; }
}