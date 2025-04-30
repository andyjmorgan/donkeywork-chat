// ------------------------------------------------------
// <copyright file="ActionModelConfiguration.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Serialization;
using DonkeyWork.Chat.Common.Models.Providers;

namespace DonkeyWork.Chat.Common.Models.Actions;

/// <summary>
/// A class representing the configuration for an action model.
/// </summary>
public class ActionModelConfiguration
{
    /// <summary>
    /// Gets or sets the provider type.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AiChatProvider ProviderType { get; set; }

    /// <summary>
    /// Gets or sets the model name.
    /// </summary>
    public string ModelName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the model should stream.
    /// </summary>
    public bool Streaming { get; set; } = true;

    /// <summary>
    /// Gets or sets additional configuration for the action model.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = [];
}
