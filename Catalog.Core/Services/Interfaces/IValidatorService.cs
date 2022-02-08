using System;
using System.Threading.Tasks;
using AzisFood.DataEngine.Abstractions.Interfaces;

namespace Catalog.Core.Services.Interfaces;

/// <summary>
///     Service to validate entity
/// </summary>
public interface IValidatorService<in T> where T : IRepoEntity
{
    /// <summary>
    ///     Validate object for errors
    /// </summary>
    /// <param name="item">Object to validate</param>
    /// <returns>Validation result</returns>
    public Task<Tuple<bool, string>> Validate(T item);
}