// ------------------------------------------------------
// <copyright file="GetAgentsItemModel.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Persistence.Agent.Repository.Agent.Models;

namespace DonkeyWork.Chat.Api.Models.Agent;

/// <summary>
/// Model for an agent item in the list.
/// </summary>
public class GetAgentsItemModel
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the execution count.
    /// </summary>
    public int ExecutionCount { get; set; }

    /// <summary>
    /// Gets or sets the tags.
    /// </summary>
    public List<string> Tags { get; set; } = [];

    /// <summary>
    /// Gets or sets the created at date.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the updated at date.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Converts a <see cref="GetAgentsItem"/> to a <see cref="GetAgentsItemModel"/>.
    /// </summary>
    /// <param name="agent">The agent.</param>
    /// <returns>A <see cref="GetAgentsItemModel"/>.</returns>
    public static GetAgentsItemModel CreateFromAgent(GetAgentsItem agent)
    {
        return new GetAgentsItemModel()
        {
            Description = agent.Description,
            ExecutionCount = agent.ExecutionCount,
            Id = agent.Id,
            Name = agent.Name,
            Tags = agent.Tags,
            CreatedAt = agent.CreatedAt,
            UpdatedAt = agent.UpdatedAt,
        };
    }
}
