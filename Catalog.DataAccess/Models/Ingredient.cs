using AzisFood.CacheService.Abstractions.Models;
using AzisFood.DataEngine.Core.Attributes;
using AzisFood.DataEngine.Mongo.Models;
using AzisFood.MQ.Abstractions.Attributes;
using AzisFood.MQ.Abstractions.Models;
using MessagePack;

namespace Catalog.DataAccess.Models;

/// <summary>
///     Model of ingredient
/// </summary>
[BusTopic(Name = "ingredient", Events = new[] {EventType.Deleted, EventType.Recache})]
// Key for demonstration how to define own name for HashSet, by default h_TypeName
[HashKey(Key = "h_Ingredient")]
[ConnectionAlias("catalog")]
[MessagePackObject]
public class Ingredient : MongoRepoEntity
{
    /// <summary>
    ///     Title of ingredient
    /// </summary>
    [Key(1)]
    public string Title { get; set; }

    /// <summary>
    ///     Identifier of measure unit
    /// </summary>
    [Key(2)]
    public string MeasureUnitId { get; set; }
}