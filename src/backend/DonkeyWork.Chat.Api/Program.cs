// ------------------------------------------------------
// <copyright file="Program.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Serialization;
using DonkeyWork.Chat.AiServices.Extensions;
using DonkeyWork.Chat.Api.Configuration;
using DonkeyWork.Chat.Api.Core.AuthenticationSchemes;
using DonkeyWork.Chat.Api.Core.Configuration;
using DonkeyWork.Chat.Api.Core.Extensions;
using DonkeyWork.Chat.Api.Core.Middleware;
using DonkeyWork.Chat.Api.Core.Services.Keycloak;
using DonkeyWork.Chat.Api.Services;
using DonkeyWork.Chat.Api.Services.Authentication;
using DonkeyWork.Chat.Api.Services.Conversation;
using DonkeyWork.Chat.Api.Workers;
using DonkeyWork.Chat.Common.Contracts;
using DonkeyWork.Chat.Common.Extensions;
using DonkeyWork.Chat.Logging.Extensions;
using DonkeyWork.Chat.McpServer.Extensions;
using DonkeyWork.Chat.Providers.Extensions;
using DonkeyWork.Persistence.Agent;
using DonkeyWork.Persistence.Agent.Extensions;
using DonkeyWork.Persistence.Chat;
using DonkeyWork.Persistence.Chat.Extensions;
using DonkeyWork.Persistence.User;
using DonkeyWork.Persistence.User.Extensions;
using DonkeyWork.Workflows.Core.Extensions;
using DonkeyWork.Workflows.Core.Scheduler.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using ServiceCollectionExtensions = DonkeyWork.Chat.AiTooling.Extensions.ServiceCollectionExtensions;

Console.WriteLine("Starting DonkeyWork.Chat.Api...");
var builder = WebApplication.CreateBuilder(args);

// Add support for environment variable configuration
builder.Configuration.AddEnvironmentVariables();

// Register services
builder.Services.AddHttpClient();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Host.AddCentralizedLogging(builder.Configuration, builder.Environment.ApplicationName);

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache(); // For session storage
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Register authentication services
builder.Services.AddScoped<IKeycloakClient, KeycloakClient>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddOptions<AllowedModelsConfiguration>()
    .BindConfiguration(nameof(AllowedModelsConfiguration))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Add session support
builder.Services.AddSession(options =>
{
    // Get cookie settings from configuration
    var cookieSettings = builder.Configuration.GetSection("CookieSettings");
    var domain = cookieSettings["Domain"];
    var secureOnly = cookieSettings.GetValue("SecureOnly", true);
    var httpOnly = cookieSettings.GetValue("HttpOnly", true);

    options.IdleTimeout = TimeSpan.FromDays(7);
    options.Cookie.Name = "DonkeyWork.Session";
    options.Cookie.HttpOnly = httpOnly;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax; // Allows redirects from identity provider
    options.Cookie.SecurePolicy = secureOnly ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;

    // Set domain if configured
    if (!string.IsNullOrWhiteSpace(domain))
    {
        options.Cookie.Domain = domain;
    }
});

// Configure data protection (for cookies)
builder.Services.AddDataProtection()
    .SetApplicationName("DonkeyWork.Chat");

// Configure authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = ApiKeyAuthenticationOptions.DefaultScheme;
        options.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DefaultScheme;
    })
    .AddCookie(options =>
    {
        // Get cookie settings from configuration
        var cookieSettings = builder.Configuration.GetSection("CookieSettings");
        var domain = cookieSettings["Domain"];
        var secureOnly = cookieSettings.GetValue("SecureOnly", true);
        var httpOnly = cookieSettings.GetValue("HttpOnly", true);

        options.Cookie.Name = "DonkeyWork.Auth";
        options.Cookie.HttpOnly = httpOnly;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = secureOnly ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;

        // Set domain if configured
        if (!string.IsNullOrWhiteSpace(domain))
        {
            options.Cookie.Domain = domain;
        }

        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
        options.LoginPath = "/api/auth/login"; // This will trigger a 401 to the frontend
        options.LogoutPath = "/api/auth/logout";
        options.AccessDeniedPath = "/api/auth/access-denied";
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
    }).AddApiKeyAuthentication();

// Configure authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(nameof(AuthorizationTypes.CookieOrApiKey), policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddAuthenticationSchemes(
            CookieAuthenticationDefaults.AuthenticationScheme,
            ApiKeyAuthenticationOptions.DefaultScheme);
    });

    var defaultPolicy = options.GetPolicy(nameof(AuthorizationTypes.CookieOrApiKey));
    if (defaultPolicy != null)
    {
        options.DefaultPolicy = defaultPolicy;
        options.FallbackPolicy = defaultPolicy;
    }
});

builder.Services.AddHostedService<TokenRefreshWorker>();
builder.Services.AddUserContext();
builder.Services.AddAiServices();
builder.Services.AddApiCoreServices();
builder.Services.AddWorkflowCore(builder.Configuration);
ServiceCollectionExtensions.AddToolServices(builder.Services);
builder.Services.AddProviderConfiguration(builder.Configuration);
builder.Services.AddScoped<IUserPostureService, UserPostureService>();
builder.Services.AddUserPersistence(builder.Configuration)
    .AddChatPersistence(builder.Configuration)
    .AddAgentPersistence(builder.Configuration);

builder.Services.AddMcpServices();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        // Get allowed origins from configuration
        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

        policy.WithOrigins(allowedOrigins ?? [])
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Important for cookies!
    });
});

// Register KeycloakConfiguration
builder.Services.Configure<KeycloakConfiguration>(
    builder.Configuration.GetSection("KeycloakConfiguration"));

// Build app
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ApiUserMiddleware>();

// todo: abstract this.
app.MapMcp();
app.MapHealthChecks("/healthz")
    .AllowAnonymous();

app.MapControllers();
app.MapScalarApiReference()
    .AllowAnonymous();

using var scope = app.Services.CreateScope();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Applying database migrations.");

try
{
    await using var context = scope.ServiceProvider.GetRequiredService<ChatPersistenceContext>();
    logger.LogInformation("Applying Chat migrations");
    await context.Database.MigrateAsync();
    logger.LogInformation("Chat migrations completed successfully.");

    await using var agentContext = scope.ServiceProvider.GetRequiredService<AgentPersistenceContext>();
    logger.LogInformation("Applying Agent migrations");
    await agentContext.Database.MigrateAsync();
    logger.LogInformation("Agent migrations completed successfully.");
    var quartzSchemaInitializer = scope.ServiceProvider.GetRequiredService<IQuartzSchemaInitializer>();
    await quartzSchemaInitializer.EnsureSchemaCreatedAsync();

    await using var userContext = scope.ServiceProvider.GetRequiredService<UserPersistenceContext>();
    logger.LogInformation("Applying User migrations");
    await userContext.Database.MigrateAsync();
    logger.LogInformation("User migrations completed successfully.");
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while applying database migrations.");
}

await app.RunAsync();
