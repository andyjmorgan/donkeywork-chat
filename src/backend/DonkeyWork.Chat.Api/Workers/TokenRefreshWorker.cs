// ------------------------------------------------------
// <copyright file="TokenRefreshWorker.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Providers.Tools;
using DonkeyWork.Chat.Providers.Services.TokenRefresh;
using DonkeyWork.Persistence.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DonkeyWork.Chat.Api.Workers;

/// <inheritdoc />
public class TokenRefreshWorker : BackgroundService
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<TokenRefreshWorker> logger;
    private readonly TokenRefreshWorkerConfiguration options;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenRefreshWorker"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="options">The token refresh options.</param>
    public TokenRefreshWorker(
        IServiceProvider serviceProvider,
        ILogger<TokenRefreshWorker> logger,
        IOptions<TokenRefreshWorkerConfiguration> options)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
        this.options = options.Value;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Token refresh background service is starting");
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                this.logger.LogInformation("Starting token refresh cycle at {Time}", DateTime.UtcNow);
                await this.RefreshTokensAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                this.logger.LogInformation("Token refresh background service is stopping");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in token refresh background service");
            }

            this.logger.LogInformation("Token refresh background service is sleeping until {Time}", DateTime.UtcNow.Add(this.options.RefreshInterval));
            await Task.Delay(this.options.RefreshInterval, cancellationToken);
        }
    }

    private static ITokenRefreshService GetTokenClient(IServiceScope scope, string providerKey)
    {
        return scope.ServiceProvider.GetRequiredKeyedService<ITokenRefreshService>(providerKey);
    }

    private async Task RefreshTokensAsync(CancellationToken cancellationToken)
    {
        using var scope = this.serviceProvider.CreateScope();
        var apiPersistenceContext = scope.ServiceProvider.GetRequiredService<UserPersistenceContext>();
        var expiryThreshold = DateTimeOffset.UtcNow.Add(this.options.RefreshThreshold);
        var userTokens = await apiPersistenceContext.UserTokens
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.ExpiresAt < expiryThreshold)
            .ToListAsync(cancellationToken);

        this.logger.LogInformation("Found {TokenCount} tokens for refresh", userTokens.Count);
        foreach (var group in userTokens.GroupBy(x => x.ProviderType))
        {
            var tokenClient = GetTokenClient(scope, group.Key.ToString());
            foreach (var token in group)
            {
                try
                {
                    if (token.Data.TryGetValue(UserProviderDataKeyType.RefreshToken, out var value))
                    {
                        this.logger.LogInformation(
                            "Refreshing token for user {UserId} for provider: {Provider}",
                            token.UserId,
                            group.Key);

                        var newToken = await tokenClient.RefreshTokenAsync(value, cancellationToken);
                        if (!string.IsNullOrWhiteSpace(newToken.RefreshToken))
                        {
                            token.Data[UserProviderDataKeyType.RefreshToken] = newToken.RefreshToken;
                        }

                        this.logger.LogInformation("Token refreshed for user {UserId}. New Expiry {Expiry}", token.UserId, newToken.ExpiresOn);
                        token.Data[UserProviderDataKeyType.AccessToken] = newToken.AccessToken;
                        var result = await apiPersistenceContext.UserTokens
                            .IgnoreQueryFilters()
                            .Where(t => t.Id == token.Id)
                            .ExecuteUpdateAsync(
                                s => s
                                    .SetProperty(t => t.Data, token.Data)
                                    .SetProperty(t => t.ExpiresAt, newToken.ExpiresOn)
                                    .SetProperty(t => t.UpdatedAt, DateTimeOffset.UtcNow)
                                    .SetProperty(t => t.Scopes, newToken.Scopes.ToList()),
                                cancellationToken);
                        if (result <= 0)
                        {
                            this.logger.LogError("Failed to update token for user {UserId}. Token not updated.", token.UserId);
                        }
                    }
                    else
                    {
                        this.logger.LogWarning("No refresh token found for user {UserId}", token.UserId);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Error while refreshing token for provider: {Provider}",  group.Key);
                    await apiPersistenceContext.UserTokens.Where(
                            x =>
                                x.Id == token.Id)
                        .ExecuteDeleteAsync(cancellationToken);
                }
            }
        }
    }
}