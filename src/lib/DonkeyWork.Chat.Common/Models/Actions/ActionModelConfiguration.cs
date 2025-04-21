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
    public AiChatProvider ProviderType { get; set; }

    /// <summary>
    /// Gets or sets the model name.
    /// </summary>
    public string ModelName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the model is a streaming model.
    /// </summary>
    public bool IsStreaming { get; set; }

    /// <summary>
    /// Gets or sets additional configuration for the action model.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = [];
}