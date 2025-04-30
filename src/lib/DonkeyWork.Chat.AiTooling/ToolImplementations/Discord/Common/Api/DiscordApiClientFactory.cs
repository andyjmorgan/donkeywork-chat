// ------------------------------------------------------
// <copyright file="DiscordApiClientFactory.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using Discord;
using Discord.Rest;
using DonkeyWork.Chat.Common.Contracts;
using DonkeyWork.Chat.Common.Models.Providers.Tools;
using Microsoft.Extensions.Logging;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.Discord.Common.Api;

/// <summary>
/// Factory for creating Discord API clients.
/// </summary>
public class DiscordApiClientFactory : IDiscordApiClientFactory
{
    private readonly ILogger<DiscordApiClientFactory> logger;
    private readonly IUserPostureService userPostureService;
    private DiscordRestClient? discordRestClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordApiClientFactory"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="userPostureService">The user posture service.</param>
    public DiscordApiClientFactory(ILogger<DiscordApiClientFactory> logger, IUserPostureService userPostureService)
    {
        this.logger = logger;
        this.userPostureService = userPostureService;
    }

    /// <inheritdoc />
    public async Task<DiscordRestClient> CreateDiscordClientAsync(CancellationToken cancellationToken = default)
    {
        if (this.discordRestClient == null)
        {
            try
            {
                var userPosture = await this.userPostureService.GetUserPosturesAsync(cancellationToken);
                var thisProvider = userPosture.UserTokens.FirstOrDefault(x => x.ProviderType == ToolProviderType.Discord);
                ArgumentNullException.ThrowIfNull(thisProvider);

                var client = new DiscordRestClient(new DiscordRestConfig
                {
                    LogLevel = LogSeverity.Info,
                    DefaultRetryMode = RetryMode.AlwaysRetry,
                });

                // Configure the client to log messages
                client.Log += (msg) =>
                {
                    var logLevel = msg.Severity switch
                    {
                        LogSeverity.Critical => LogLevel.Critical,
                        LogSeverity.Error => LogLevel.Error,
                        LogSeverity.Warning => LogLevel.Warning,
                        LogSeverity.Info => LogLevel.Information,
                        LogSeverity.Verbose => LogLevel.Debug,
                        LogSeverity.Debug => LogLevel.Trace,
                        _ => LogLevel.Information
                    };

                    this.logger.Log(logLevel, msg.Exception, "{Source}: {Message}", msg.Source, msg.Message);
                    return Task.CompletedTask;
                };

                // Login with the token from user posture
                await client.LoginAsync(TokenType.Bearer, thisProvider.Keys[UserProviderDataKeyType.AccessToken]);
                this.discordRestClient = client;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error creating Discord client");
                throw;
            }
        }

        return this.discordRestClient;
    }
}