// ------------------------------------------------------
// <copyright file="ToolProviderDefinition.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Models.Providers.Tools.GenericProvider;
using DonkeyWork.Chat.Common.Models.Providers.Tools.GenericProvider.Implementations;

namespace DonkeyWork.Chat.Common.Models.Providers.Tools;

/// <summary>
/// A tool provider definition.
/// </summary>
public class ToolProviderDefinition
{
    /// <summary>
    /// Gets or sets the provider type.
    /// </summary>
    public ToolProviderType ProviderType { get; set; }

    /// <summary>
    /// Gets or sets the authorization type.
    /// </summary>
    public ToolProviderAuthorizationType AuthorizationType { get; set; }

    /// <summary>
    /// Gets or sets the provider name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the provider description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the provider icon.
    /// </summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the provider configuration if required.
    /// </summary>
    public BaseGenericProviderConfiguration? ProviderConfiguration { get; set; }

    /// <summary>
    /// Gets or sets the Application definitions.
    /// </summary>
    public Dictionary<ToolProviderApplicationType, ToolProviderApplicationDefinition> Applications { get; set; } = [];

    /// <summary>
    /// A static list of tool providers.
    /// </summary>
    /// <returns>A dictionary of all tool providers.</returns>
    public static Dictionary<ToolProviderType, ToolProviderDefinition> GetToolProviders()
    {
        return new Dictionary<ToolProviderType, ToolProviderDefinition>()
        {
            {
                ToolProviderType.Microsoft, new ToolProviderDefinition()
                {
                    ProviderType = ToolProviderType.Microsoft,
                    Description = "Access Microsoft services like Outlook, OneDrive, Todo and more",
                    Applications = new Dictionary<ToolProviderApplicationType, ToolProviderApplicationDefinition>()
                    {
                        {
                            ToolProviderApplicationType.MicrosoftIdentity, new ToolProviderApplicationDefinition()
                            {
                                Description = "Microsoft identity services",
                                Icon = "pi-microsoft",
                                Name = "Microsoft Identity",
                                Application = ToolProviderApplicationType.MicrosoftIdentity,
                                Provider = ToolProviderType.Microsoft,
                            }
                        },
                        {
                            ToolProviderApplicationType.MicrosoftOutlook, new ToolProviderApplicationDefinition()
                            {
                                Description = "Microsoft outlook services",
                                Icon = "https://res-1.cdn.office.net/files/fabric-cdn-prod_20240610.001/assets/brand-icons/product/svg/outlook_16x1.svg",
                                Name = "Microsoft Outlook",
                                Application = ToolProviderApplicationType.MicrosoftOutlook,
                                Provider = ToolProviderType.Microsoft,
                            }
                        },
                        {
                            ToolProviderApplicationType.MicrosoftTodo, new ToolProviderApplicationDefinition()
                            {
                                Description = "Microsoft Todo list services",
                                Icon = "https://res-1.cdn.office.net/files/fabric-cdn-prod_20240610.001/assets/brand-icons/product/svg/todo_16x1.svg",
                                Name = "Microsoft Todo",
                                Application = ToolProviderApplicationType.MicrosoftTodo,
                                Provider = ToolProviderType.Microsoft,
                            }
                        },
                        {
                            ToolProviderApplicationType.MicrosoftOneDrive, new ToolProviderApplicationDefinition()
                            {
                                Description = "Microsoft OneDrive services",
                                Icon = "https://res-1.cdn.office.net/files/fabric-cdn-prod_20240610.001/assets/brand-icons/product/svg/onedrive_16x1.svg",
                                Name = "Microsoft OneDrive",
                                Application = ToolProviderApplicationType.MicrosoftTodo,
                                Provider = ToolProviderType.Microsoft,
                            }
                        },
                    },
                    Icon = "pi-microsoft",
                    AuthorizationType = ToolProviderAuthorizationType.OAuth,
                    Name = "Microsoft",
                }
            },
            {
                ToolProviderType.Google, new ToolProviderDefinition()
                {
                    ProviderType = ToolProviderType.Google,
                    Description = "Access Google services like Gmail, Google drive, and more",
                    Applications = new Dictionary<ToolProviderApplicationType, ToolProviderApplicationDefinition>()
                    {
                        {
                            ToolProviderApplicationType.GoogleIdentity,
                            new ToolProviderApplicationDefinition()
                            {
                                Description = "Google identity services",
                                Icon = "http://ssl.gstatic.com/images/branding/product/1x/contacts_2022_64dp.png",
                                Name = "Google Identity",
                                Application = ToolProviderApplicationType.GoogleIdentity,
                                Provider = ToolProviderType.Google,
                            }
                        },
                        {
                            ToolProviderApplicationType.GoogleMail,
                            new ToolProviderApplicationDefinition()
                            {
                                Description = "Google mail services",
                                Icon = "http://ssl.gstatic.com/images/branding/product/1x/gmail_2020q4_64dp.png",
                                Name = "Gmail",
                                Application = ToolProviderApplicationType.GoogleMail,
                                Provider = ToolProviderType.Google,
                            }
                        },
                        {
                            ToolProviderApplicationType.GoogleDrive,
                            new ToolProviderApplicationDefinition()
                            {
                                Description = "Google mail services",
                                Icon = "http://ssl.gstatic.com/images/branding/product/1x/drive_2020q4_64dp.png",
                                Name = "Google Drive",
                                Application = ToolProviderApplicationType.GoogleMail,
                                Provider = ToolProviderType.Google,
                            }
                        },
                    },
                    Icon = "pi-google",
                    AuthorizationType = ToolProviderAuthorizationType.OAuth,
                    Name = "Google",
                }
            },
            {
                ToolProviderType.Discord, new ToolProviderDefinition()
                {
                    ProviderType = ToolProviderType.Discord,
                    Description = "Access to your Discord account",
                    Applications = new Dictionary<ToolProviderApplicationType, ToolProviderApplicationDefinition>()
                    {
                        {
                            ToolProviderApplicationType.Discord, new ToolProviderApplicationDefinition()
                            {
                                Description = "Discord services",
                                Icon = "pi-discord",
                                Name = "Discord",
                                Provider = ToolProviderType.Discord,
                                Application = ToolProviderApplicationType.Discord,
                            }
                        },
                    },
                    Icon = "pi-discord",
                    AuthorizationType = ToolProviderAuthorizationType.OAuth,
                    Name = "Discord",
                }
            },
            {
                ToolProviderType.Serp, new ToolProviderDefinition()
                {
                    ProviderType = ToolProviderType.Serp,
                    Description = "Access to serp search services",
                    Applications = new Dictionary<ToolProviderApplicationType, ToolProviderApplicationDefinition>()
                    {
                        {
                            ToolProviderApplicationType.Serp, new ToolProviderApplicationDefinition()
                            {
                                Description = "Serp search services",
                                Icon = "https://serpapi.com/apple-touch-icon.png",
                                Name = "Serp Search",
                                Provider = ToolProviderType.Discord,
                                Application = ToolProviderApplicationType.Discord,
                            }
                        },
                    },
                    Icon = "https://serpapi.com/apple-touch-icon.png",
                    AuthorizationType = ToolProviderAuthorizationType.Static,
                    Name = "Serp Api",
                    ProviderConfiguration = new SerpApiConfiguration(),
                }
            },
            {
                ToolProviderType.Swarmpit, new ToolProviderDefinition()
                {
                    ProviderType = ToolProviderType.Swarmpit,
                    Description = "Access to swarmpit services",
                    Applications = new Dictionary<ToolProviderApplicationType, ToolProviderApplicationDefinition>()
                    {
                        {
                            ToolProviderApplicationType.Swarmpit, new ToolProviderApplicationDefinition()
                            {
                                Description = "Swarmpit services",
                                Icon = "https://swarmpit.io/img/apple-touch-icon.png",
                                Name = "Swarmpit",
                                Provider = ToolProviderType.Swarmpit,
                                Application = ToolProviderApplicationType.Swarmpit,
                            }
                        },
                    },
                    Icon = "https://swarmpit.io/img/apple-touch-icon.png",
                    AuthorizationType = ToolProviderAuthorizationType.Static,
                    Name = "SwarmPit",
                    ProviderConfiguration = new SwarmpitConfiguration(),
                }
            },
        };
    }
}