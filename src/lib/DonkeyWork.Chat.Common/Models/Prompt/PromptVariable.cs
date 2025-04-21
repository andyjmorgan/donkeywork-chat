// ------------------------------------------------------
// <copyright file="PromptVariable.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Prompt;

/// <summary>
/// Gets or sets the prompt variable.
/// </summary>
public class PromptVariable
{
    /// <summary>
    /// Gets or sets the variable name.
    /// </summary>
    required public string Name { get; set; }

    /// <summary>
    /// Gets or sets the variable description.
    /// </summary>
    required public string Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the variable is required.
    /// </summary>
    required public string IsRequired { get; set; }

    /// <summary>
    /// Gets or sets an optional default value for the variable.
    /// </summary>
    public string? DefaultValue { get; set; }
}