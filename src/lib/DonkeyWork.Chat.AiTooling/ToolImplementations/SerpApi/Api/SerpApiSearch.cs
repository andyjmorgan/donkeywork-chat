// ------------------------------------------------------
// <copyright file="SerpApiSearch.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Collections;
using System.Text.Json;
using DonkeyWork.Chat.Common.Models.Providers.Tools.GenericProvider.Implementations;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SerpApi;

namespace DonkeyWork.Chat.AiTooling.ToolImplementations.SerpApi.Api;

/// <inheritdoc />
public class SerpApiSearch : ISerpApiSearch
{
    /// <summary>
    /// The serp api configuration.
    /// </summary>
    private readonly SerpApiConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="SerpApiSearch"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public SerpApiSearch(IOptions<SerpApiConfiguration> options)
    {
        this.configuration = options.Value;
    }

    /// <summary>
    /// Searches google.
    /// </summary>
    /// <param name="query">The query string.</param>
    /// <returns>A json document.</returns>
    public JsonDocument SearchGoogle(string query)
    {
        Hashtable ht = new Hashtable();
        ht.Add("q", query);
        ht.Add("hl", "en");
        ht.Add("google_domain", "google.com");
        GoogleSearch search = new GoogleSearch(ht, this.configuration.ApiKey);
        return JsonDocument.Parse(search.GetJson().ToString(Formatting.None));
    }
}