// ------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.UserContext;
using Microsoft.Extensions.DependencyInjection;

namespace DonkeyWork.Chat.Common.Extensions;

/// <summary>
/// A service collection extension for adding the user context.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the user context.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>A <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddUserContext(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUserContextProvider, UserContextProvider>();
        return serviceCollection;
    }
}