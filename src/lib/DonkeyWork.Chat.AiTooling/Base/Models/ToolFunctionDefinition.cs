// ------------------------------------------------------
// <copyright file="ToolFunctionDefinition.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Serialization;

namespace DonkeyWork.Chat.AiTooling.Base.Models;

/// <summary>
/// A Tool Function Definition.
/// </summary>
public record ToolFunctionDefinition
{
    /// <summary>
    /// gets the tool type.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = "object";

    /// <summary>
    /// Gets the required properties.
    /// </summary>
    [JsonPropertyName("required")]
    public List<string> Required { get; init; } = [];

    /// <summary>
    /// Gets the parameters.
    /// </summary>
    [JsonPropertyName("properties")]
    public Dictionary<string, ToolFunctionParameterDefinition> Properties { get; init; } = new ();
}