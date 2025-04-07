// ------------------------------------------------------
// <copyright file="MicrosoftGraphSerializationOptions.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.MicrosoftGraph.Common;

/// <summary>
/// A class that contains the serialization options for Microsoft Graph.
/// </summary>
public static class MicrosoftGraphSerializationOptions
{
    /// <summary>
    /// A static instance of <see cref="JsonSerializerOptions"/> for Microsoft Graph.
    /// </summary>
    public static readonly JsonSerializerOptions MicrosoftGraphJsonSerializerOptions = new ()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };
}