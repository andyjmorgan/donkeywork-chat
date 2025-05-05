// ------------------------------------------------------
// <copyright file="GetAgentsModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Api.Models.Agent;

/// <summary>
/// Model for getting a list of agents.
/// </summary>
public class GetAgentsModel
{
    /// <summary>
    /// Gets or sets the total count.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the agents.
    /// </summary>
    public List<GetAgentsItemModel> Agents { get; set; } = [];
}