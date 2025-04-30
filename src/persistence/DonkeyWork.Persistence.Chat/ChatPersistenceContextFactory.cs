// ------------------------------------------------------
// <copyright file="ChatPersistenceContextFactory.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Services.UserContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DonkeyWork.Persistence.Chat;

/// <summary>
/// A factory for creating the api context.
/// </summary>
public class ChatPersistenceContextFactory : IDesignTimeDbContextFactory<ChatPersistenceContext>
{
    /// <summary>
    /// Custom logic for allowing migrations to be run.
    /// </summary>
    /// <param name="args">The args.</param>
    /// <returns>The Policy context.</returns>
    public ChatPersistenceContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ChatPersistenceContext>();
        optionsBuilder.UseNpgsql(
            $"Host=localhost;Port=5532;Database={nameof(ChatPersistenceContext)};Username=donkeywork;Password=donkeywork;");

        return new ChatPersistenceContext(optionsBuilder.Options, new UserContextProvider());
    }
}
