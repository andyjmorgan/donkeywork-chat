// ------------------------------------------------------
// <copyright file="GetActionsResponseItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Persistence.Agent.Repository.Action.Models;

/// <summary>
/// Response for getting actions.
/// </summary>
public class GetActionsResponseItem
{
    /// <summary>
    /// Gets or sets the total count.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the actions.
    /// </summary>
    public List<ActionItem> Actions { get; set; } = [];
}