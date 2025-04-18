// ------------------------------------------------------
// <copyright file="ApiKeyController.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Api.Models.ApiKey;
using DonkeyWork.Chat.Common.UserContext;
using DonkeyWork.Chat.Persistence.Common;
using DonkeyWork.Chat.Persistence.Repository.ApiKey;
using DonkeyWork.Chat.Persistence.Repository.ApiKey.Models;
using Microsoft.AspNetCore.Mvc;

namespace DonkeyWork.Chat.Api.Controllers;

/// <summary>
/// Api key controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ApiKeyController : ControllerBase
{
    /// <summary>
    /// The api key repository.
    /// </summary>
    private readonly IApiKeyRepository apiKeyRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyController"/> class.
    /// </summary>
    /// <param name="apiKeyRepository">The api key repository.</param>
    public ApiKeyController(IApiKeyRepository apiKeyRepository)
    {
        this.apiKeyRepository = apiKeyRepository;
    }

    /// <summary>
    /// Gets the users api keys.
    /// </summary>
    /// <param name="pagingParameters">The paging parameters.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetApiKeysModel))]
    public async Task<IActionResult> GetApiKeys([FromQuery] PagingParameters pagingParameters)
    {
        var apiKeys = await this.apiKeyRepository.GetApiKeysAsync(pagingParameters, this.HttpContext.RequestAborted);
        return this.Ok(new GetApiKeysModel()
        {
            Count = apiKeys.TotalCount,
            ApiKeys = apiKeys.Items.Select(x => new ApiKeySummaryModel()
            {
                Id = x.Id,
                Description = x.Description,
                CreatedAt = x.CreatedAt,
                ApiKey = x.HashApiKey(),
                Name = x.Name,
                IsEnabled = x.IsEnabled,
            }),
        });
    }

    /// <summary>
    /// Gets a specific api key including the key itself.
    /// </summary>
    /// <param name="id">The api key id.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiKeyModel))]
    public async Task<IActionResult> GetApiKey([FromRoute] Guid id)
    {
        var apiKey = await this.apiKeyRepository.GetApiKeyAsync(id, this.HttpContext.RequestAborted);

        return apiKey is null
            ? this.NotFound()
            : this.Ok(new ApiKeyModel()
            {
                Id = id,
                Description = apiKey.Description,
                Name = apiKey.Name,
                IsEnabled = apiKey.IsEnabled,
                ApiKey = apiKey.ApiKey,
            });
    }

    /// <summary>
    /// Creates an api key for the specified API key.
    /// </summary>
    /// <param name="model">The api key model.</param>
    /// <param name="userContextProvider">The user provider.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateApiKey(
        [FromBody] UpsertApiKeyModel model,
        [FromServices]IUserContextProvider userContextProvider)
    {
        await this.apiKeyRepository.CreateApiKeyAsync(new ApiKeyItem(userContextProvider.UserId.ToString())
        {
            Description = model.Description,
            IsEnabled = model.IsEnabled,
            Name = model.Name,
        });
        return this.Created();
    }

    /// <summary>
    /// Creates an api key for the specified API key.
    /// </summary>
    /// <param name="id">The api key id.</param>
    /// <param name="model">The updated model.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> PatchApiKey(
        [FromRoute] Guid id,
        [FromBody] UpsertApiKeyModel model)
    {
        var apiKey = await this.apiKeyRepository.UpdateApiKeyAsync(
            id,
            new ApiKeyItem()
            {
                Description = model.Description,
                Name = model.Name,
                IsEnabled = model.IsEnabled,
            }, this.HttpContext.RequestAborted);
        return apiKey is null ? this.NotFound() : this.NoContent();
    }
}