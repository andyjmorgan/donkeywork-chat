// ------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.McpServer.Handlers;
using ModelContextProtocol.Protocol.Types;
using ModelContextProtocol.Server;

namespace DonkeyWork.Chat.McpServer.Extensions;

/// <summary>
/// A service collection extension for adding the McpServer services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Gets a default implementation of the server info.
    /// </summary>
    private static Implementation DefaultImplementation => new Implementation()
    {
        Name = "DonkeyWork.McpServer",
        Version = "1.0.0",
    };

    /// <summary>
    /// Adds AI services.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>A <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddMcpServices(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<IMcpToolsHandler, McpToolsHandler>()
            .AddScoped<IMcpPromptsHandler, McpPromptsHandler>()
            .AddMcpServer()
            .WithHttpTransport(htpTransportOptions =>
            {
                htpTransportOptions.ConfigureSessionOptions =
                    (HttpContext context, McpServerOptions options, CancellationToken _) =>
                    {
                        options.ServerInfo = DefaultImplementation;
                        return Task.CompletedTask;
                    };
            })
            .WithListToolsHandler(async (context, cancellationToken) =>
            {
                var handler = GetToolHandler(context);
                return await handler.HandleListAsync(context, cancellationToken);
            })
            .WithCallToolHandler(async (context, cancellationToken) =>
            {
                var handler = GetToolHandler(context);
                return await handler.HandleCallAsync(context, cancellationToken);
            })
            .WithListPromptsHandler(async (context, cancellationToken) =>
            {
                var handler = GetPromptHandler(context);
                return await handler.HandleListAsync(context, cancellationToken);
            })
            .WithGetPromptHandler(async (context, cancellationToken) =>
            {
                var handler = GetPromptHandler(context);
                return await handler.HandleGetAsync(context, cancellationToken);
            });

        return serviceCollection;
    }

    private static IMcpToolsHandler GetToolHandler<T>(RequestContext<T> context)
    {
        var handler = context.Services?.GetRequiredService<IMcpToolsHandler>();
        if (handler == null)
        {
            throw new Exception($"Handler not found for type: {nameof(T)}");
        }

        return handler;
    }
    
    private static IMcpPromptsHandler GetPromptHandler<T>(RequestContext<T> context)
    {
        var handler = context.Services?.GetRequiredService<IMcpPromptsHandler>();
        if (handler == null)
        {
            throw new Exception($"Handler not found for type: {nameof(T)}");
        }

        return handler;
    }
}