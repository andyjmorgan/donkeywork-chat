// ------------------------------------------------------
// <copyright file="ApiPersistenceContextFactory.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.UserContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DonkeyWork.Chat.Persistence;

/// <summary>
/// A factory for creating the vector storage context.
/// </summary>
public class ApiPersistenceContextFactory : IDesignTimeDbContextFactory<ApiPersistenceContext>
{
    /// <summary>
    /// Custom logic for allowing migrations to be run.
    /// </summary>
    /// <param name="args">The args.</param>
    /// <returns>The Policy context.</returns>
    public ApiPersistenceContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApiPersistenceContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5532;Database=donkeyworkChat;Username=donkeywork;Password=donkeywork;");

        return new ApiPersistenceContext(optionsBuilder.Options, new UserContextProvider());
    }
}