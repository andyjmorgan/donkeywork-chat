// ------------------------------------------------------
// <copyright file="BasePromptItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Persistence.Common.Repository.Base;

namespace DonkeyWork.Persistence.Agent.Repository.Prompt.Models;

/// <summary>
/// A base prompt item.
/// </summary>
public abstract record BasePromptItem : BaseItem
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
    /// Gets the usage count.
    /// </summary>
    public int UsageCount { get; init; }
}