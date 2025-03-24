// ------------------------------------------------------
// <copyright file="GetUserInformationResponse.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Api.Models;

/// <summary>
/// A response containing user information.
/// </summary>
public record GetUserInformationResponse
{
    /// <summary>
    /// Gets the user's unique identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the user's first name.
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the user's family name.
    /// </summary>
    public string FamilyName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the user's username.
    /// </summary>
    public string UserName { get; init; } = string.Empty;

    /// <summary>
    /// gets the users email address.
    /// </summary>
    public string EmailAddress { get; init; } = string.Empty;
}