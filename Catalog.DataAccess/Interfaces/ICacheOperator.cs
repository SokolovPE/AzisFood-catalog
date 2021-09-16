using System;
using System.Threading.Tasks;

namespace Catalog.DataAccess.Interfaces
{
    /// <summary>
    /// Perform operations with cache for entity
    /// </summary>
    /// <typeparam name="T">Tpe of entity</typeparam>
    public interface ICacheOperator<T>
    {
        /// <summary>
        /// Fully refresh cache of entity
        /// </summary>
        /// <param name="expiry">Key expiration time</param>
        /// <param name="asHash">Save as hashset</param>
        Task FullRecache(TimeSpan expiry, bool asHash = true);
    }
}