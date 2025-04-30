// ------------------------------------------------------
// <copyright file="ChatStartFragment.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Chat.Common.Models.Streaming.Chat;

/// <summary>
/// A chat start fragment.
/// </summary>
public record ChatStartFragment : BaseChatFragment
{
    /// <summary>
    /// Gets the model name.
    /// </summary>
    public string ModelName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the model message provider id.
    /// </summary>
    public string MessageProviderId { get; init; } = string.Empty;
}