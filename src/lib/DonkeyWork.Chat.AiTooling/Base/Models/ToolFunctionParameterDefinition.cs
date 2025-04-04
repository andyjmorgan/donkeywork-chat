// ------------------------------------------------------
// <copyright file="ToolFunctionParameterDefinition.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json.Serialization;

namespace DonkeyWork.Chat.AiTooling.Base.Models;

/// <summary>
/// A tool function parameter definition.
/// </summary>
public record ToolFunctionParameterDefinition
{
    /// <summary>
    /// Gets the type.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// Gets the enum values.
    /// </summary>
    [JsonPropertyName("enum")]
    public List<string>? Enum { get; init; }

    /// <summary>
    /// Gets the items schema if this parameter is an array.
    /// </summary>
    [JsonPropertyName("items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ToolFunctionParameterDefinition? Items { get; init; }

    /// <summary>
    /// Gets the description.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;
}