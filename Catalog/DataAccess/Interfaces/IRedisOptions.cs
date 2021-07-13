namespace Catalog.DataAccess.Interfaces
{
    public interface IRedisOptions
    {
        string ConnectionString { get; set; }
    }
}