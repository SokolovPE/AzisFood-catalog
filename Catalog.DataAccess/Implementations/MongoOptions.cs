using Catalog.DataAccess.Interfaces;

namespace Catalog.DataAccess.Implementations
{
    public class MongoOptions : IMongoOptions
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}