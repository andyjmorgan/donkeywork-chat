// ------------------------------------------------------
// <copyright file="ActionJob.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Workflows.Core.Actions.Models;
using DonkeyWork.Workflows.Core.Actions.Services.ActionConsumer;
using Microsoft.Extensions.Logging;
using Quartz;

namespace DonkeyWork.Workflows.Core.Scheduler.Jobs;

/// <summary>
/// A user action job.
/// </summary>
public class ActionJob : IJob
{
    private readonly ILogger<ActionJob> logger;
    private readonly IActionExecutionPublisherService publisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionJob"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="publisher">The publisher.</param>
    public ActionJob(ILogger<ActionJob> logger, IActionExecutionPublisherService publisher)
    {
        this.logger = logger;
        this.publisher = publisher;
    }

    /// <inheritdoc />
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            // Extract the ActionExecutionRequest from the job data map
            var jobDataMap = context.MergedJobDataMap;
            var actionId = jobDataMap.GetGuid("Id");
            var userId = jobDataMap.GetGuid("UserId");

            var request = new ActionExecutionRequest
            {
                Id = actionId,
                UserId = userId,
            };

            this.logger.LogInformation(
                "Executing action {ActionId} for user {UserId}, execution ID: {ExecutionId}",
                request.Id,
                request.UserId,
                request.ExecutionId);

            await this.publisher.PublishAsync(request, context.CancellationToken);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error executing action job");
        }
    }
}
