// ------------------------------------------------------
// <copyright file="ActionOrchestratorService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using System.Runtime.CompilerServices;
using DonkeyWork.Chat.Common.Models.Actions;
using DonkeyWork.Chat.Common.Models.Streaming;
using DonkeyWork.Chat.Common.Models.Streaming.Exceptions;
using DonkeyWork.Persistence.Agent.Repository.Action;
using DonkeyWork.Persistence.Agent.Repository.ActionExecution;
using DonkeyWork.Workflows.Core.Actions.Models;
using DonkeyWork.Workflows.Core.Actions.Services.ActionExecution;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace DonkeyWork.Workflows.Core.Actions.Services.ActionOrchestrator;

/// <inheritdoc />
public class ActionOrchestratorService : IActionOrchestratorService
{
    private readonly ILogger<ActionOrchestratorService> logger;
    private readonly IActionRepository actionRepository;
    private readonly IActionExecutionRepository actionExecutionRepository;
    private readonly IActionExecutionService actionExecutionService;
    private readonly ResiliencePipeline resiliencePipeline;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionOrchestratorService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="actionRepository">The action repository.</param>
    /// <param name="actionExecutionRepository">The action execution repository.</param>
    /// <param name="actionExecutionService">The action execution service.</param>
    public ActionOrchestratorService(
        ILogger<ActionOrchestratorService> logger,
        IActionRepository actionRepository,
        IActionExecutionRepository actionExecutionRepository,
        IActionExecutionService actionExecutionService)
    {
        this.logger = logger;
        this.actionRepository = actionRepository;
        this.actionExecutionRepository = actionExecutionRepository;
        this.actionExecutionService = actionExecutionService;
        this.resiliencePipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Exponential,
                MaxRetryAttempts = 3,
                Name = nameof(ActionOrchestratorService),
                OnRetry = args =>
                {
                    this.logger.LogWarning(
                        "Retry attempt {RetryAttempt} after failure when executing action. Error: {Error}",
                        args.AttemptNumber,
                        args.Outcome.Exception?.Message);
                    return ValueTask.CompletedTask;
                },
            })
            .AddTimeout(TimeSpan.FromMinutes(5))
            .Build();
    }

    /// <inheritdoc />
    public async Task ExecuteActionAsync(
        ActionExecutionRequest actionExecutionRequest,
        CancellationToken cancellationToken = default)
    {
        await this.resiliencePipeline.ExecuteAsync(
            async token =>
        {
            var actionExecutionId = await this.actionExecutionRepository.CreateTaskAsync(actionExecutionRequest.ExecutionId, actionExecutionRequest.Id, actionExecutionRequest.ActionName, cancellationToken);
            var action = await this.actionRepository.GetActionByIdForExecutionAsync(actionExecutionRequest.Id, token);
            if (action == null)
            {
                this.logger.LogError("Action with id {ActionId} not found.", actionExecutionRequest.Id);
                return;
            }

            action.ExecutionId = actionExecutionRequest.ExecutionId;
            List<BaseStreamItem> results = [];

            // Set the status to pending.
            await this.actionExecutionRepository.SetTaskRunningAsync(actionExecutionId, token);
            try
            {
                await foreach (var result in this.actionExecutionService.ExecuteActionAsync(action, actionExecutionRequest.ActionInput, token))
                {
                    results.Add(result);
                    if (result is ExceptionResult exceptionResult)
                    {
                        this.logger.LogError(
                            exceptionResult.Exception,
                            "An exception was detected while executing the action {ActionId}",
                            actionExecutionRequest.Id);
                        throw exceptionResult.Exception;
                    }
                }

                // Set the status.
                await this.actionExecutionRepository.SetTaskCompletedStatusAsync(
                    actionExecutionId,
                    results,
                    ActionExecutionStatus.Completed,
                    token);
                this.logger.LogInformation(
                    "Action {ActionName} with id {ActionId} completed successfully.",
                    action.Name,
                    actionExecutionRequest.Id);
            }
            catch (Exception ex)
            {
                // Set the status to failed.
                await this.actionExecutionRepository.SetTaskCompletedStatusAsync(
                    actionExecutionId,
                    results,
                    ActionExecutionStatus.Failed,
                    token);
                this.logger.LogError(
                    ex,
                    "Failed to execute action {ActionName} with id {ActionId}.",
                    action.Name,
                    actionExecutionRequest.Id);
                throw; // Rethrow to trigger retry if applicable
            }
        }, cancellationToken);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<BaseStreamItem> ExecuteActionStreamAsync(
        ActionExecutionRequest actionExecutionRequest,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var action =
            await this.actionRepository.GetActionByIdForExecutionAsync(actionExecutionRequest.Id, cancellationToken);
        if (action != null)
        {
            List<BaseStreamItem> results = [];

            await foreach (var result in this.actionExecutionService.ExecuteActionAsync(action, actionExecutionRequest.ActionInput, cancellationToken))
            {
                results.Add(result);
                yield return result;
                if (result is ExceptionResult exceptionResult)
                {
                    this.logger.LogError(
                        exceptionResult.Exception,
                        "An exception was detected while executing the action {ActionId}",
                        actionExecutionRequest.Id);
                    throw exceptionResult.Exception;
                }
            }

            // Set the status.
            await this.actionExecutionRepository.SetTaskCompletedStatusAsync(
                actionExecutionRequest.Id,
                results,
                ActionExecutionStatus.Completed,
                cancellationToken);
            this.logger.LogInformation(
                "Action {ActionName} with id {ActionId} completed successfully.",
                action.Name,
                actionExecutionRequest.Id);
        }
    }
}
