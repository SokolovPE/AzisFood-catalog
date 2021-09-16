using Catalog.DataAccess.Interfaces;

namespace Catalog.DataAccess.Implementations
{
    public class MQOptions : IMQOptions
    {
        public string ConnectionString { get; set; }
    }
}