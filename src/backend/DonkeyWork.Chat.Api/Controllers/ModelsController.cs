// ------------------------------------------------------
// <copyright file="ModelsController.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using DonkeyWork.Chat.Api.Configuration;
using DonkeyWork.Chat.Api.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DonkeyWork.Chat.Api.Controllers;

/// <summary>
/// A models controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ModelsController(IOptions<AllowedModelsConfiguration> modelConfiguration)
    : ControllerBase
{
    /// <summary>
    /// Gets the models.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AllowedModelsResponse))]
    public IActionResult GetModels()
    {
        return this.Ok(new AllowedModelsResponse()
        {
            AllowedModels = modelConfiguration.Value.AllowedModels,
            DefaultModel = new KeyValuePair<string, string>(modelConfiguration.Value.AllowedModels.First().Key, modelConfiguration.Value.AllowedModels.First().Value.First()),
        });
    }
}