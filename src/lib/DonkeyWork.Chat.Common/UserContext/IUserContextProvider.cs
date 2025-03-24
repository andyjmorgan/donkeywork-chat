// ------------------------------------------------------
// <copyright file="IUserContextProvider.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.UserContext;

/// <summary>
/// The user context.
/// </summary>
public interface IUserContextProvider
{
    /// <summary>
    /// Gets the user id.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Sets the user id.
    /// </summary>
    /// <param name="userId">The user id.</param>
    public void SetUserId(Guid userId);
}