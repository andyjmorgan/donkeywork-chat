// ------------------------------------------------------
// <copyright file="GoogleApiClientFactory.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Contracts;
using DonkeyWork.Chat.Common.Models.Providers.Tools;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Gmail.v1;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.GoogleApi.Common.Api;

/// <summary>
/// A Google api client factory.
/// </summary>
public class GoogleApiClientFactory : IGoogleApiClientFactory
{
    /// <summary>
    /// The google provider agent name.
    /// </summary>
    private static readonly string AgentName = "DonkeyWorkTooling";

    private readonly HttpClient httpClient;
    private readonly IUserPostureService userPostureService;

    /// <summary>
    /// A gmail service.
    /// </summary>
    private GmailService? gmailService = null;

    /// <summary>
    /// A drive service.
    /// </summary>
    private DriveService? driveService = null;

    /// <summary>
    /// A google credential.
    /// </summary>
    private GoogleCredential? googleCredential;

    /// <summary>
    /// An oauth2 service.
    /// </summary>
    private Oauth2Service? oauth2Service = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleApiClientFactory"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The http client factory.</param>
    /// <param name="userPostureService">The user posture service.</param>
    public GoogleApiClientFactory(IHttpClientFactory httpClientFactory, IUserPostureService userPostureService)
    {
        this.httpClient = httpClientFactory.CreateClient(nameof(ToolProviderType.Google));
        this.userPostureService = userPostureService;
    }

    /// <inheritdoc />
    public async Task<GmailService> CreateGmailServiceAsync(CancellationToken cancellationToken = default)
    {
        if (this.gmailService is not null)
        {
            return this.gmailService;
        }

        await this.ValidateCredentialsAsync(cancellationToken);
        this.gmailService = new GmailService(
            new BaseClientService.Initializer
            {
                HttpClientInitializer = this.googleCredential,
                ApplicationName = AgentName,
            });
        return this.gmailService;
    }

    /// <inheritdoc />
    public async Task<DriveService> CreateDriveServiceAsync(
        CancellationToken cancellationToken = default)
    {
        if (this.driveService is not null)
        {
            return this.driveService;
        }

        await this.ValidateCredentialsAsync(cancellationToken);
        this.driveService = new DriveService(
            new BaseClientService.Initializer
            {
                HttpClientInitializer = this.googleCredential,
                ApplicationName = AgentName,
            });
        return this.driveService;
    }

    /// <inheritdoc />
    public async Task<Oauth2Service> CreateOauth2ServiceAsync(CancellationToken cancellationToken = default)
    {
        if (this.oauth2Service is not null)
        {
            return this.oauth2Service;
        }

        await this.ValidateCredentialsAsync(cancellationToken);
        this.oauth2Service = new Oauth2Service(
            new BaseClientService.Initializer
            {
                HttpClientInitializer = this.googleCredential,
                ApplicationName = AgentName,
            });
        return this.oauth2Service;
    }

    private async Task ValidateCredentialsAsync(CancellationToken cancellationToken)
    {
        if (this.googleCredential is null)
        {
            var userPostures = await this.userPostureService.GetUserPostureAsync(ToolProviderType.Google, cancellationToken);
            ArgumentNullException.ThrowIfNull(userPostures);
            this.googleCredential = GoogleCredential.FromAccessToken(userPostures.Keys[UserProviderDataKeyType.AccessToken]);
        }
    }
}
