// ------------------------------------------------------
// <copyright file="UserContextProvider.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.UserContext;

/// <inheritdoc />
public class UserContextProvider : IUserContextProvider
{
    /// <summary>
    /// Gets the user Id.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Sets the user Id.
    /// </summary>
    /// <param name="userId">The user id.</param>
    public void SetUserId(Guid userId)
    {
        this.UserId = userId;
    }
}