using System.Collections.Generic;
using System.Threading.Tasks;
using AzisFood.DataEngine.Interfaces;
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
        /// <returns>List of entities</returns>
        Task<List<TDto>> GetAllAsync();

        /// <summary>
        /// Get entities from catalog by id
        /// </summary>
        /// <param name="id">Identifier of entity</param>
        /// <returns>Instance of entity</returns>
        Task<TDto> GetByIdAsync(string id);

        /// <summary>
        /// Add new entity to catalog
        /// </summary>
        /// <param name="product">Entity to add</param>
        /// <returns>Insertion result</returns>
        Task<TDto> AddAsync(TRequestDto product);

        /// <summary>
        /// Update entity in catalog
        /// </summary>
        /// <param name="id">Identifier of entity to be updated</param>
        /// <param name="product">Entity to be updated</param>
        /// <returns>Insertion result</returns>
        Task UpdateAsync(string id, TRequestDto product);

        /// <summary>
        /// Delete entity from catalog
        /// </summary>
        /// <param name="id">Identifier of entity to be deleted</param>
        /// <returns>Deletion result</returns>
        Task<T> DeleteAsync(string id);

        /// <summary>
        /// Delete entities from catalog
        /// </summary>
        /// <param name="ids">Array with identifiers of entities to be deleted</param>
        /// <returns>Deletion result</returns>
        Task DeleteAsync(string[] ids);
    }
}