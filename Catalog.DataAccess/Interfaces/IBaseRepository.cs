using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.DataAccess.Interfaces
{
    public interface IBaseRepository<TEntity>
    {
        /// <summary>
        /// Get items async.
        /// </summary>
        /// <returns>Collection of item.</returns>
        public Task<List<TEntity>> GetAsync();
        /// <summary>
        /// Get item by id.
        /// </summary>
        /// <param name="id">Identifier of item.</param>
        /// <returns>Item with supplied id.</returns>
        Task<TEntity> GetAsync(string id);
        /// <summary>
        /// Insert new item.
        /// </summary>
        /// <param name="item">Item to insert.</param>
        /// <returns>Inserted item.</returns>
        Task<TEntity> CreateAsync(TEntity item);
        /// <summary>
        /// Update item by id.
        /// </summary>
        /// <param name="id">Id of item.</param>
        /// <param name="itemIn">New value.</param>
        Task UpdateAsync(string id, TEntity itemIn);
        /// <summary>
        /// Delete item.
        /// </summary>
        /// <param name="itemIn">Value to delete.</param>
        Task RemoveAsync(TEntity itemIn);
        /// <summary>
        /// Delete item by id.
        /// </summary>
        /// <param name="id">Id of item to delete.</param>
        Task RemoveAsync(string id);
        /// <summary>
        /// Delete items with id in list.
        /// </summary>
        /// <param name="ids">Ids of items to delete.</param>
        Task RemoveManyAsync(string[] ids);
    }
}