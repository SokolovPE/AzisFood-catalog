using Catalog.DataAccess.Interfaces;

namespace Catalog.DataAccess.Implementations
{
    public class RedisOptions : IRedisOptions
    {
        public string ConnectionString { get; set; }
    }
}