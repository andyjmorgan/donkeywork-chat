// ------------------------------------------------------
// <copyright file="IPromptRepository.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Persistence.Common;
using DonkeyWork.Chat.Persistence.Repository.Conversation.Models;
using DonkeyWork.Chat.Persistence.Repository.Prompt.Models;

namespace DonkeyWork.Chat.Persistence.Repository.Prompt;

/// <summary>
/// A prompt repository.
/// </summary>
public interface IPromptRepository
{
    /// <summary>
    /// Gets all prompts.
    /// </summary>
    /// <param name="pagingParameters">The paging parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A response of <see cref="GetConversationsResponse"/>.</returns>
    public Task<GetPromptsResponseItem> GetPromptsAsync(PagingParameters pagingParameters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a prompt by title.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<PromptContentItem?> GetPromptContentItemAsync(string title, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a prompt.
    /// </summary>
    /// <param name="id">The prompt id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A response of <see cref="GetConversationsResponse"/>.</returns>
    public Task<PromptContentItem?> GetPromptContentAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a conversation.
    /// </summary>
    /// <param name="id">The prompt id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<bool> DeletePromptAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a prompt.
    /// </summary>
    /// <param name="id">The prompt Id.</param>
    /// <param name="prompt">The prompt.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<bool> UpdatePromptAsync(Guid id, UpsertPromptItem prompt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a prompt.
    /// </summary>
    /// <param name="prompt">The prompt.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<bool> AddPromptAsync(UpsertPromptItem prompt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Increases the prompts usage count.
    /// </summary>
    /// <param name="id">The prompt id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task IncreasePromptUsageCountAsync(Guid id, CancellationToken cancellationToken = default);
}