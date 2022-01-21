using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzisFood.DataEngine.Abstractions.Interfaces;
using Catalog.Sdk.Models;

namespace Catalog.Core.Services.Interfaces
{
    /// <summary>
    /// Interface of service which operate entity
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    /// <typeparam name="TDto">Type of entity Dto</typeparam>
    /// <typeparam name="TRequestDto">Type of entity request Dto</typeparam>
    public interface IService<T, TDto, in TRequestDto> 
        where T: IRepoEntity 
        where TDto: IDto
        where TRequestDto: IRequestDto
    {
        /// <summary>
        /// Get all entities presented in catalog
        /// </summary>
        /// <param name="token">Token for operation cancel</param>
        /// <returns>List of entities</returns>
        Task<List<TDto>> GetAllAsync(CancellationToken token = default);

        /// <summary>
        /// Get entities from catalog by id
        /// </summary>
        /// <param name="id">Identifier of entity</param>
        /// <param name="token">Token for operation cancel</param>
        /// <returns>Instance of entity</returns>
        Task<TDto> GetByIdAsync(Guid id, CancellationToken token = default);

        /// <summary>
        /// Add new entity to catalog
        /// </summary>
        /// <param name="product">Entity to add</param>
        /// <param name="token">Token for operation cancel</param>
        /// <returns>Insertion result</returns>
        Task<TDto> AddAsync(TRequestDto product, CancellationToken token = default);

        /// <summary>
        /// Update entity in catalog
        /// </summary>
        /// <param name="id">Identifier of entity to be updated</param>
        /// <param name="product">Entity to be updated</param>
        /// <param name="token">Token for operation cancel</param>
        /// <returns>Insertion result</returns>
        Task UpdateAsync(Guid id, TRequestDto product, CancellationToken token = default);

        /// <summary>
        /// Delete entity from catalog
        /// </summary>
        /// <param name="id">Identifier of entity to be deleted</param>
        /// <param name="token">Token for operation cancel</param>
        /// <returns>Deletion result</returns>
        Task<T> DeleteAsync(Guid id, CancellationToken token = default);

        /// <summary>
        /// Delete entities from catalog
        /// </summary>
        /// <param name="ids">Array with identifiers of entities to be deleted</param>
        /// <param name="token">Token for operation cancel</param>
        /// <returns>Deletion result</returns>
        Task DeleteAsync(Guid[] ids, CancellationToken token = default);
    }
}