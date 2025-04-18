// ------------------------------------------------------
// <copyright file="ApiKeyItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Text;

namespace DonkeyWork.Chat.Persistence.Repository.ApiKey.Models;

/// <summary>
/// An api key item.
/// </summary>
public record ApiKeyItem
{
    private static readonly string ApiKeyPrefix = "DW";

    /// <summary>
    /// Gets the api key name.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the api key description.
    /// </summary>
    public string? Description { get; init; } = null;

    /// <summary>
    /// Gets a value indicating whether the API key is enabled.
    /// </summary>
    public bool IsEnabled { get; init; } = true;

    /// <summary>
    /// Gets the api key.
    /// </summary>
    public string ApiKey { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyItem"/> class.
    /// </summary>
    /// <param name="userId">The user's id.</param>
    public ApiKeyItem(string userId)
    {
        this.ApiKey = $"{ApiKeyPrefix}_{Convert.ToBase64String(Encoding.UTF8.GetBytes($"{DateTime.UtcNow:O}_{userId}"))}";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyItem"/> class.
    /// </summary>
    public ApiKeyItem()
    {
        this.ApiKey = string.Empty;
    }

    /// <summary>
    /// Hashes the API key for display purposes.
    /// </summary>
    /// <returns>A secure string.</returns>
    public string HashApiKey()
    {
        string firstThree = this.ApiKey.Substring(0, 3);
        string lastThree = this.ApiKey.Substring(this.ApiKey.Length - 3);
        string middle = new string('*', 6);
        return $"{firstThree}{middle}{lastThree}";
    }
}