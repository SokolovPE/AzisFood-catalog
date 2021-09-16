using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.DataAccess.Interfaces
{
    public interface ICachedBaseRepository<TEntity> : IBaseRepository<TEntity>
    {
        /// <summary>
        /// Get items async from hashset
        /// </summary>
        /// <returns>Collection of item</returns>
        public Task<IEnumerable<TEntity>> GetHashAsync();
        
        /// <summary>
        /// Get item by id async from hashset by id
        /// </summary>
        /// <param name="id">Identifier of item</param>
        /// <returns>Item with supplied id</returns>
        public Task<TEntity> GetHashAsync(string id);
    }
}