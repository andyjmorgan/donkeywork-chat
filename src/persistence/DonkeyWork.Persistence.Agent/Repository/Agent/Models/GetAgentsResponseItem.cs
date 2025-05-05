// ------------------------------------------------------
// <copyright file="GetAgentsResponseItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Persistence.Agent.Repository.Agent.Models;

/// <summary>
/// Response for getting agents.
/// </summary>
public class GetAgentsResponseItem
{
    /// <summary>
    /// Gets or sets the total count.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the agents.
    /// </summary>
    public List<GetAgentsItem> Agents { get; set; } = [];
}