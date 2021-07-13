using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.DataAccess.Interfaces;
using Catalog.Extensions;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Catalog.DataAccess.Implementations
{
    public class RedisCacheService : IRedisCacheService
    {
        private static Lazy<ConnectionMultiplexer> _lazyConnection;
        private static readonly object Locker = new();
        public static ConnectionMultiplexer Connection => _lazyConnection.Value;
        
        public RedisCacheService(IRedisOptions options)
        {
            lock (Locker)
            {
                _lazyConnection ??=
                    new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options.ConnectionString));
            }
        }

        public async Task<bool> SetAsync(RedisKey key, RedisValue value, TimeSpan? expiry, CommandFlags flags = CommandFlags.FireAndForget)
        {
            return await Connection.GetDatabase().StringSetAsync(key, value, expiry, When.Always, flags);
        }
        
        public async Task<bool> SetAsync<T>(RedisKey key, T value, TimeSpan? expiry, CommandFlags flags = CommandFlags.FireAndForget)
        {
            var jsonValue = JsonConvert.SerializeObject(value);
            return await Connection.GetDatabase().StringSetAsync(key, jsonValue, expiry, When.Always, flags);
        }

        public async Task<RedisValue> GetAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await Connection.GetDatabase().StringGetAsync(key, flags);
        }

        public async Task<T> GetAsync<T>(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            var result = await Connection.GetDatabase().StringGetAsync(key, flags);
            return result == RedisValue.Null ? default : JsonConvert.DeserializeObject<T>(result);
        }
        
        public async Task<bool> RemoveAsync(RedisKey key, CommandFlags flags = CommandFlags.FireAndForget)
        {
            return await Connection.GetDatabase().KeyDeleteAsync(key, flags);
        }

        // public async Task HashSetAsync(RedisKey key, IEnumerable value, CommandFlags flags = CommandFlags.FireAndForget)
        // {
        //     var hashValue = value.ToHashEntryList();
        //     await Connection.GetDatabase().HashSetAsync(key, hashValue.ToArray(), flags);
        // }
        
        // public async Task<IEnumerable<T>> HashGetAsync<T>(RedisKey key, IEnumerable value, CommandFlags flags = CommandFlags.FireAndForget)
        // {
        //     var result = await Connection.GetDatabase().HashGetAllAsync(key, flags);
        //     var x = result.ToEnumerable<T>();
        // }
    }
}