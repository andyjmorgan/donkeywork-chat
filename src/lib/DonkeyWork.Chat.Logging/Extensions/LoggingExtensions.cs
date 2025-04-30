// ------------------------------------------------------
// <copyright file="LoggingExtensions.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DonkeyWork.Chat.Logging.Extensions
{
    /// <summary>
    /// A class containing extension methods for configuring centralized logging.
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// Adds centralized logging to the host builder using Serilog.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="applicationName">The application name.</param>
        /// <returns>A <see cref="IHostBuilder"/>.</returns>
        public static IHostBuilder AddCentralizedLogging(this IHostBuilder builder, ConfigurationManager configurationManager, string applicationName)
        {
            var basePath = System.Diagnostics.Debugger.IsAttached
                ? $"{AppContext.BaseDirectory}appsettings.logging.json"
                : "appsettings.logging.json";

            configurationManager.AddJsonFile(basePath, optional: false, reloadOnChange: true);
            return builder.UseSerilog((hostingContext, services, loggerConfiguration) =>
            {
                // Then apply host configuration which may override defaults
                loggerConfiguration
                    .ReadFrom.Configuration(configurationManager)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", applicationName);
            });
        }
    }
}