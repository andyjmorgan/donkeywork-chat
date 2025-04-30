// ------------------------------------------------------
// <copyright file="IPromptRepository.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Persistence.Agent.Repository.Prompt.Models.ActionPrompt;
using DonkeyWork.Persistence.Agent.Repository.Prompt.Models.SystemPrompt;
using DonkeyWork.Persistence.Common.Common;

namespace DonkeyWork.Persistence.Agent.Repository.Prompt;

/// <summary>
/// A prompt repository.
/// </summary>
public interface IPromptRepository
{
    /// <summary>
    /// Gets all system prompts.
    /// </summary>
    /// <param name="pagingParameters">The paging parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A response of <see cref="GetPromptsResponseItem"/>.</returns>
    public Task<GetPromptsResponseItem> GetPromptsAsync(PagingParameters pagingParameters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all action prompts.
    /// </summary>
    /// <param name="pagingParameters">The paging parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A response of <see cref="GetActionPromptsResponseItem"/>.</returns>
    public Task<GetActionPromptsResponseItem> GetActionPromptsAsync(PagingParameters pagingParameters, CancellationToken cancellationToken = default);

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
    /// <returns>A response of <see cref="PromptContentItem"/>.</returns>
    public Task<PromptContentItem?> GetPromptContentAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an action prompt by title.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<ActionPromptItem?> GetActionPromptContentItemAsync(string title, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an action prompt.
    /// </summary>
    /// <param name="id">The prompt id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A response of <see cref="ActionPromptItem"/>.</returns>
    public Task<ActionPromptItem?> GetActionPromptContentAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a prompt.
    /// </summary>
    /// <param name="id">The prompt id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<bool> DeletePromptAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an action prompt.
    /// </summary>
    /// <param name="id">The prompt id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<bool> DeleteActionPromptAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a prompt.
    /// </summary>
    /// <param name="id">The prompt Id.</param>
    /// <param name="prompt">The prompt.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<bool> UpdatePromptAsync(Guid id, UpsertPromptItem prompt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an action prompt.
    /// </summary>
    /// <param name="id">The prompt Id.</param>
    /// <param name="prompt">The prompt.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<bool> UpdateActionPromptAsync(Guid id, UpsertActionPromptItem prompt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a prompt.
    /// </summary>
    /// <param name="prompt">The prompt.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<bool> AddPromptAsync(UpsertPromptItem prompt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an action prompt.
    /// </summary>
    /// <param name="prompt">The prompt.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<bool> AddActionPromptAsync(UpsertActionPromptItem prompt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Increases the prompts usage count.
    /// </summary>
    /// <param name="id">The prompt id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task IncreasePromptUsageCountAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Increases the action prompts usage count.
    /// </summary>
    /// <param name="id">The prompt id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task IncreaseActionPromptUsageCountAsync(Guid id, CancellationToken cancellationToken = default);
}