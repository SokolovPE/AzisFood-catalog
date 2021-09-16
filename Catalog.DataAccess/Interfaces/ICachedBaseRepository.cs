using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.DataAccess.Interfaces
{
    public interface ICachedBaseRepository<TEntity> : IBaseRepository<TEntity>
    {
        /// <summary>
        /// Get items async
        /// </summary>
        /// <returns>Collection of item</returns>
        public Task<IEnumerable<TEntity>> GetHashAsync();
    }
}