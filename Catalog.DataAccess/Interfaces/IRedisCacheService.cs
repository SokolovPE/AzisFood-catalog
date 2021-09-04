using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Catalog.DataAccess.Interfaces
{
    /// <summary>
    /// Service to operate cache in redis
    /// </summary>
    public interface IRedisCacheService
    {
        /// <summary>
        /// Redis connection
        /// </summary>
        static ConnectionMultiplexer Connection;
        
        /// <summary>
        /// Set value to redis cache
        /// </summary>
        /// <param name="key">The key of the string</param>
        /// <param name="value">The value to set</param>
        /// <param name="expiry">The expiry to set</param>
        /// <param name="flags">Flags of operation</param>
        /// <returns>Status of set operation</returns>
        Task<bool> SetAsync(RedisKey key, RedisValue value, TimeSpan? expiry,
            CommandFlags flags = CommandFlags.FireAndForget);

        /// <summary>
        /// Set json converted value to redis cache
        /// </summary>
        /// <param name="key">The key of the string</param>
        /// <param name="value">The value to set</param>
        /// <param name="expiry">The expiry to set</param>
        /// <param name="flags">Flags of operation</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>Status of set operation</returns>
        Task<bool> SetAsync<T>(RedisKey key, T value, TimeSpan? expiry,
            CommandFlags flags = CommandFlags.FireAndForget);
        
        /// <summary>
        /// Get value from redis cache
        /// </summary>
        /// <param name="key">The key of the string</param>
        /// <param name="flags">Flags of operation</param>
        /// <returns>Value from redis</returns>
        Task<RedisValue> GetAsync(RedisKey key, CommandFlags flags = CommandFlags.None);
        
        /// <summary>
        /// Get value from redis cache
        /// </summary>
        /// <param name="key">The key of the string</param>
        /// <param name="flags">Flags of operation</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>Value from redis</returns>
        Task<T> GetAsync<T>(RedisKey key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Remove value from redis cache
        /// </summary>
        /// <param name="key">The key of the string</param>
        /// <param name="flags">Flags of operation</param>
        /// <returns>Status of set operation</returns>
        Task<bool> RemoveAsync(RedisKey key, CommandFlags flags = CommandFlags.FireAndForget);
    }
}