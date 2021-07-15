namespace Catalog.DataAccess.Interfaces
{
    //TODO: Singleton instead?
    public interface IMongoOptions
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}