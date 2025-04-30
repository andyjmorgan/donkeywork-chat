// ------------------------------------------------------
// <copyright file="ActionExecutionHostService.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Common.Services.UserContext;
using DonkeyWork.Workflows.Core.Actions.Models;
using DonkeyWork.Workflows.Core.Actions.Services.ActionOrchestrator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DonkeyWork.Workflows.Core.Actions.Services.ActionConsumer;

/// <summary>
/// Background service that hosts worker tasks to process action execution requests.
/// This service creates worker tasks that each run in their own scope.
/// </summary>
public sealed class ActionExecutionHostService : IActionExecutionHostService
{
    /// <summary>
    /// Service that provides access to the central action queue.
    /// </summary>
    private readonly IActionExecutionQueueService queueService;

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<ActionExecutionHostService> logger;

    /// <summary>
    /// Service provider factory used to create scoped service providers.
    /// </summary>
    private readonly IServiceScopeFactory serviceScopeFactory;

    /// <summary>
    /// Cancellation token source used to signal cancellation to worker tasks.
    /// </summary>
    private readonly CancellationTokenSource cancellationTokenSource = new ();

    /// <summary>
    /// Collection of background tasks responsible for processing queued action requests.
    /// </summary>
    private readonly List<Task> workerTasks;

    /// <summary>
    /// Flag indicating whether this instance has been disposed.
    /// </summary>
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionExecutionHostService"/> class.
    /// Creates multiple worker tasks based on the number of logical processors available.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="queueService">The queue service providing access to the central queue.</param>
    /// <param name="serviceScopeFactory">The factory used to create scoped service providers.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public ActionExecutionHostService(
        ILogger<ActionExecutionHostService> logger,
        IActionExecutionQueueService queueService,
        IServiceScopeFactory serviceScopeFactory)
    {
        this.logger = logger;
        this.queueService = queueService ??
                             throw new ArgumentNullException(nameof(queueService));

        this.serviceScopeFactory = serviceScopeFactory ??
                                    throw new ArgumentNullException(nameof(serviceScopeFactory));

        // Determine number of workers based on CPU count or specified count
        int taskCount = Math.Max(1, Environment.ProcessorCount / 2);

        // Initialize worker tasks
        this.workerTasks = new List<Task>(taskCount);
        for (int i = 0; i < taskCount; i++)
        {
            this.workerTasks.Add(Task.Run(() => this.ProcessQueueAsync(this.cancellationTokenSource.Token)));
        }
    }

    /// <summary>
    /// Gets the number of worker tasks processing the queue.
    /// </summary>
    public int WorkerCount => this.workerTasks.Count;

    /// <inheritdoc />
    public void Publish(ActionExecutionRequest request)
    {
        this.queueService.Publish(request);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        // Cancel all worker tasks
        this.cancellationTokenSource.Cancel();

        try
        {
            // Wait for all worker tasks to complete with a timeout
            Task.WaitAll(this.workerTasks.ToArray(), TimeSpan.FromSeconds(30));
        }
        catch (AggregateException)
        {
            // Ignore task cancellation exceptions.
        }
        finally
        {
            // Dispose resources
            this.cancellationTokenSource.Dispose();

            this.disposed = true;
        }
    }

    /// <summary>
    /// Processes action execution requests from the queue asynchronously.
    /// This method runs in a background task and continues until cancellation is requested.
    /// Each iteration creates a new scope to properly manage scoped dependencies.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task ProcessQueueAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            ActionExecutionRequest? request;

            try
            {
                // Try to get the next request from the queue
                if (!this.queueService.TryTake(out request, 100, cancellationToken))
                {
                    await Task.Delay(10, cancellationToken); // Short delay to prevent CPU spinning
                    continue;
                }

                if (request is null)
                {
                    await Task.Delay(10, cancellationToken); // Short delay to prevent CPU spinning
                    continue;
                }

                // Create a new scope for processing this request
                using var scope = this.serviceScopeFactory.CreateScope();
                var userContextProvider = scope.ServiceProvider.GetRequiredService<IUserContextProvider>();
                userContextProvider.SetUserId(request.UserId);
                var orchestratorService = scope.ServiceProvider.GetRequiredService<IActionOrchestratorService>();

                // Process the request within the scope
                await orchestratorService.ExecuteActionAsync(request, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Expected exception when cancellation is requested, exit the loop
                break;
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                this.logger.LogError(ex, "Error processing action");
            }
        }
    }
}
