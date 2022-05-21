using AzisFood.CacheService.Abstractions.Models;
using AzisFood.DataEngine.Postgres.Models;
using AzisFood.MQ.Abstractions.Attributes;
using AzisFood.MQ.Abstractions.Models;
using MessagePack;

namespace Catalog.DataAccess.Models;

/// <summary>
///     Model of product option
/// </summary>
[BusTopic(Name = "option", Events = new[] {EventType.Deleted, EventType.Recache})]
// Key for demonstration how to define own name for HashSet, by default h_TypeName
[HashKey(Key = "h_Option")]
public class Option : PgRepoEntity<CatalogDbContext>
{
    /// <summary>
    ///     Title of option
    /// </summary>
    [Key(1)]
    public string Title { get; set; }

    /// <summary>
    ///     Url of image cover
    /// </summary>
    [Key(2)]
    public string ImageUrl { get; set; }

    /// <summary>
    ///     Price of option
    /// </summary>
    [Key(3)]
    public decimal Price { get; set; }
}