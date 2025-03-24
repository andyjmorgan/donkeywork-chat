// ------------------------------------------------------
// <copyright file="ISerpApiSearch.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text.Json;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.SerpApi.Api;

/// <summary>
/// The serp api search interface.
/// </summary>
public interface ISerpApiSearch
{
    /// <summary>
    /// Searches google.
    /// </summary>
    /// <param name="query">The query string.</param>
    /// <returns>A <see cref="JsonDocument"/>.</returns>
    public JsonDocument SearchGoogle(string query);
}