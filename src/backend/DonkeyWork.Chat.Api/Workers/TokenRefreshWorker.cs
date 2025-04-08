// ------------------------------------------------------
// <copyright file="TokenRefreshWorker.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Providers;
using DonkeyWork.Chat.Persistence;
using DonkeyWork.Chat.Providers.Services.TokenRefresh;
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
        try
        {
            this.logger.LogInformation("Starting token refresh cycle at {Time}", DateTime.UtcNow);
            await this.RefreshTokensAsync(cancellationToken);
            await Task.Delay(this.options.RefreshInterval, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            this.logger.LogInformation("Token refresh background service is stopping");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error in token refresh background service");
        }
    }

    private static ITokenRefreshService GetTokenClient(IServiceScope scope, string providerKey)
    {
        return scope.ServiceProvider.GetRequiredKeyedService<ITokenRefreshService>(providerKey);
    }

    private async Task RefreshTokensAsync(CancellationToken cancellationToken)
    {
        using var scope = this.serviceProvider.CreateScope();
        var apiPersistenceContext = scope.ServiceProvider.GetRequiredService<ApiPersistenceContext>();
        var expiryThreshold = DateTimeOffset.UtcNow.Add(-this.options.RefreshThreshold);
        var userTokens = await apiPersistenceContext.UserTokens
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.ExpiresAt < expiryThreshold)
            .ToListAsync(cancellationToken);
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
                        token.Data[UserProviderDataKeyType.RefreshToken] = newToken.RefreshToken;
                        token.Data[UserProviderDataKeyType.AccessToken] = newToken.AccessToken;
                        await apiPersistenceContext.UserTokens
                            .IgnoreQueryFilters()
                            .Where(t => t.Id == token.Id)
                            .ExecuteUpdateAsync(
                                s => s
                                    .SetProperty(t => t.Data, token.Data)
                                    .SetProperty(t => t.ExpiresAt, token.ExpiresAt)
                                    .SetProperty(t => t.UpdatedAt, DateTimeOffset.UtcNow)
                                    .SetProperty(t => t.Scopes, newToken.Scopes.ToList()),
                                cancellationToken);
                    }
                    else
                    {
                        this.logger.LogWarning("No refresh token found for user {UserId}", token.UserId);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Error while refreshing token for provider: {Provider}",  group.Key);
                }
            }
        }
    }
}