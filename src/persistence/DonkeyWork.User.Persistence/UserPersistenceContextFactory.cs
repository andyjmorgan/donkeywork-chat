// ------------------------------------------------------
// <copyright file="ApiPersistenceContextFactory.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Commonn.Services.UserContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DonkeyWork.Chat.Persistence;

/// <summary>
/// A factory for creating the api context.
/// </summary>
public class ApiPersistenceContextFactory : IDesignTimeDbContextFactory<UserPersistenceContext>
{
    /// <summary>
    /// Custom logic for allowing migrations to be run.
    /// </summary>
    /// <param name="args">The args.</param>
    /// <returns>The Policy context.</returns>
    public UserPersistenceContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UserPersistenceContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5532;Database=donkeyworkChat;Username=donkeywork;Password=donkeywork;");

        return new UserPersistenceContext(optionsBuilder.Options, new UserContextProvider());
    }
}