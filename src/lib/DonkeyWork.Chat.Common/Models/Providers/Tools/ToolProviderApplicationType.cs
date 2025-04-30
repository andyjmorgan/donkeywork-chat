// ------------------------------------------------------
// <copyright file="ToolProviderApplicationType.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Reflection;

namespace DonkeyWork.Chat.Common.Models.Providers.Tools;

/// <summary>
/// An enumeration of tool provider application types.
/// </summary>
public enum ToolProviderApplicationType
{
    /// <summary>
    /// The Google Identity application.
    /// </summary>
    [ToolProviderType(ToolProviderType.Google)]
    GoogleIdentity,

    /// <summary>
    /// The Google Drive application.
    /// </summary>
    [ToolProviderType(ToolProviderType.Google)]
    GoogleDrive,

    /// <summary>
    /// The Google Mail application.
    /// </summary>
    [ToolProviderType(ToolProviderType.Google)]
    GoogleMail,

    /// <summary>
    /// The Google Calendar application.
    /// </summary>
    [ToolProviderType(ToolProviderType.Google)]
    GoogleCalendar,

    /// <summary>
    /// The Microsoft Identity application.
    /// </summary>
    [ToolProviderType(ToolProviderType.Microsoft)]
    MicrosoftIdentity,

    /// <summary>
    /// The Microsoft Outlook application.
    /// </summary>
    [ToolProviderType(ToolProviderType.Microsoft)]
    MicrosoftOutlook,

    /// <summary>
    /// The Microsoft Excel application.
    /// </summary>
    [ToolProviderType(ToolProviderType.Microsoft)]
    MicrosoftOneDrive,

    /// <summary>
    /// THe Microsoft Todo application.
    /// </summary>
    [ToolProviderType(ToolProviderType.Microsoft)]
    MicrosoftTodo,

    /// <summary>
    /// The Discord application.
    /// </summary>
    [ToolProviderType(ToolProviderType.Discord)]
    Discord,

    /// <summary>
    /// The swarmpit application.
    /// </summary>
    [ToolProviderType(ToolProviderType.Swarmpit)]
    Swarmpit,

    /// <summary>
    /// The serp application.
    /// </summary>
    [ToolProviderType(ToolProviderType.Serp)]
    Serp,

    /// <summary>
    /// A delay tool.
    /// </summary>
    [ToolProviderType(ToolProviderType.BuiltIn)]
    Delay,

    /// <summary>
    /// A date time tool.
    /// </summary>
    [ToolProviderType(ToolProviderType.BuiltIn)]
    DateTime,
}

/// <summary>
/// An enumeration of tool provider applications.
/// </summary>
public static class ToolProviderApplicationsExtensions
{
    /// <summary>
    /// Gets the provider for the application.
    /// </summary>
    /// <param name="app">The app.</param>
    /// <returns>A <see cref="ToolProviderType"/>.</returns>
    public static ToolProviderType GetProvider(this ToolProviderApplicationType app)
    {
        var field = typeof(ToolProviderApplicationType).GetField(app.ToString());
        var attribute = field?.GetCustomAttribute<ToolProviderTypeAttribute>();
        return attribute?.Provider ?? ToolProviderType.Unknown;
    }

    /// <summary>
    /// Gets the applications for the provider.
    /// </summary>
    /// <param name="provider">The provider.</param>
    /// <returns>A list of <see cref="ToolProviderApplicationType"/>.</returns>
    public static List<ToolProviderApplicationType> GetApps(this ToolProviderType provider)
    {
        return Enum.GetValues<ToolProviderApplicationType>()
            .Where(app => app.GetProvider() == provider)
            .ToList();
    }
}
