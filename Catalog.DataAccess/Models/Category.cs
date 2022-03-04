using System;
using AzisFood.DataEngine.Core.Attributes;
using AzisFood.DataEngine.Mongo.Models;
using AzisFood.MQ.Abstractions.Attributes;
using MessagePack;

namespace Catalog.DataAccess.Models;

/// <summary>
///     Model of product category
/// </summary>
[BusTopic(Name = "category")]
[ConnectionAlias("catalog")]
public class Category : MongoRepoEntity
{
    /// <summary>
    ///     Title of category
    /// </summary>
    [Key(1)]
    public string Title { get; set; }

    /// <summary>
    ///     Possible subcategories
    /// </summary>
    [Key(2)]
    public Guid[] SubCategories { get; set; }

    /// <summary>
    ///     Order of category
    /// </summary>
    [Key(3)]
    public int Order { get; set; }
}