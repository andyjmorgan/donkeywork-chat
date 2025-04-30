// ------------------------------------------------------
// <copyright file="AgentPersistenceContextFactory.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Services.UserContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DonkeyWork.Persistence.Agent;

/// <summary>
/// A factory for creating the agent context.
/// </summary>
public class AgentPersistenceContextFactory : IDesignTimeDbContextFactory<AgentPersistenceContext>
{
    /// <summary>
    /// Custom logic for allowing migrations to be run.
    /// </summary>
    /// <param name="args">The args.</param>
    /// <returns>The Policy context.</returns>
    public AgentPersistenceContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AgentPersistenceContext>();
        optionsBuilder.UseNpgsql(
            $"Host=localhost;Port=5532;Database={nameof(AgentPersistenceContext)};Username=donkeywork;Password=donkeywork;");

        return new AgentPersistenceContext(optionsBuilder.Options, new UserContextProvider());
    }
}