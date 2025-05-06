// ------------------------------------------------------
// <copyright file="AgentRepository.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Persistence.Agent.Entity.Agent;
using DonkeyWork.Persistence.Agent.Repository.Agent.Models;
using DonkeyWork.Persistence.Common.Common;
using Microsoft.EntityFrameworkCore;

namespace DonkeyWork.Persistence.Agent.Repository.Agent;

/// <inheritdoc />
public class AgentRepository(AgentPersistenceContext persistenceContext)
    : IAgentRepository
{
    /// <inheritdoc />
    public async Task<GetAgentsResponseItem> GetAgentsAsync(
        PagingParameters pagingParameters,
        CancellationToken cancellationToken = default)
    {
        var agentQuery = persistenceContext.Agents
            .AsNoTracking()
            .AsQueryable();

        if (pagingParameters.Offset > 0)
        {
            agentQuery = agentQuery.Skip(pagingParameters.Offset);
        }

        var count = await agentQuery.CountAsync(cancellationToken);
        var agents = await agentQuery
            .OrderBy(a => a.Name)
            .Take(pagingParameters.Limit)
            .Select(a => new GetAgentsItem
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                Tags = a.Tags,
                ExecutionCount = a.ExecutionCount,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
            })
            .ToListAsync(cancellationToken);

        return new GetAgentsResponseItem
        {
            TotalCount = count,
            Agents = agents,
        };
    }

    /// <inheritdoc />
    public async Task<AgentItem?> GetAgentByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var agent = await persistenceContext.Agents
            .AsNoTracking()
            .SingleOrDefaultAsync(a => a.Name == name, cancellationToken);

        if (agent == null)
        {
            return null;
        }

        return MapToAgentItem(agent);
    }

    /// <inheritdoc />
    public async Task<AgentItem?> GetAgentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var agent = await persistenceContext.Agents
            .AsNoTracking()
            .SingleOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (agent == null)
        {
            return null;
        }

        return MapToAgentItem(agent);
    }

    /// <inheritdoc />
    public async Task<AgentItem?> GetAgentByIdForExecutionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var agent = await persistenceContext.Agents
            .SingleOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (agent == null)
        {
            return null;
        }

        return MapToAgentItem(agent);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAgentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var agent = await persistenceContext.Agents
            .SingleOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (agent == null)
        {
            return false;
        }

        persistenceContext.Agents.Remove(agent);
        await persistenceContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAgentAsync(Guid id, UpsertAgentItem agentData, CancellationToken cancellationToken = default)
    {
        var agent = await persistenceContext.Agents
            .SingleOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (agent == null)
        {
            await this.AddAgentAsync(agentData, cancellationToken);
            return true;
        }

        // Update properties
        agent.Name = agentData.Name;
        agent.Description = agentData.Description;
        agent.Tags = agentData.Tags;
        agent.Nodes = agentData.Nodes;
        agent.NodeEdges = agentData.NodeEdges;

        await persistenceContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public async Task<Guid> AddAgentAsync(UpsertAgentItem agentData, CancellationToken cancellationToken = default)
    {
        var agent = new AgentEntity
        {
            Id = agentData.Id ?? Guid.NewGuid(),
            Name = agentData.Name,
            Description = agentData.Description,
            Tags = agentData.Tags,
            Nodes = agentData.Nodes,
            NodeEdges = agentData.NodeEdges,
            ExecutionCount = 0,
        };

        await persistenceContext.Agents.AddAsync(agent, cancellationToken);
        await persistenceContext.SaveChangesAsync(cancellationToken);

        return agent.Id;
    }

    /// <inheritdoc />
    public async Task<bool> IncrementExecutionCountAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var agent = await persistenceContext.Agents
            .SingleOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (agent == null)
        {
            return false;
        }

        agent.ExecutionCount++;
        await persistenceContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static AgentItem MapToAgentItem(AgentEntity agent)
    {
        return new AgentItem
        {
            Id = agent.Id,
            Name = agent.Name,
            Description = agent.Description,
            Tags = agent.Tags,
            Nodes = agent.Nodes,
            NodeEdges = agent.NodeEdges,
            ExecutionCount = agent.ExecutionCount,
            CreatedAt = agent.CreatedAt,
            UpdatedAt = agent.UpdatedAt,
        };
    }
}
