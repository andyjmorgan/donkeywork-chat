// ------------------------------------------------------
// <copyright file="ConditionItem.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

namespace DonkeyWork.Persistence.Agent.Entity.Agent.Models;

/// <summary>
/// A condition item.
/// </summary>
public class ConditionItem
{
    /// <summary>
    /// Gets or sets the condition item id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the condition expression.
    /// </summary>
    required public string Expression { get; set; }
}
