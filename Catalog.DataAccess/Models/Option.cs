using AzisFood.CacheService.Abstractions.Models;
using AzisFood.DataEngine.Core.Attributes;
using AzisFood.DataEngine.Mongo.Models;
using AzisFood.MQ.Abstractions.Attributes;
using AzisFood.MQ.Abstractions.Models;

namespace Catalog.DataAccess.Models;

/// <summary>
///     Model of product option
/// </summary>
[BusTopic(Name = "option", Events = new[] {EventType.Deleted, EventType.Recache})]
// Key for demonstration how to define own name for HashSet, by default h_TypeName
[HashKey(Key = "h_Option")]
[ConnectionAlias("catalog")]
public class Option : MongoRepoEntity
{
    /// <summary>
    ///     Title of option
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    ///     Url of image cover
    /// </summary>
    public string ImageUrl { get; set; }

    /// <summary>
    ///     Price of option
    /// </summary>
    public decimal Price { get; set; }
}