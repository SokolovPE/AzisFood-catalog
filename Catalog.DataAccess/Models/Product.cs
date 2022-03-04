using System;
using AzisFood.CacheService.Abstractions.Models;
using AzisFood.DataEngine.Core.Attributes;
using AzisFood.DataEngine.Mongo.Models;
using AzisFood.MQ.Abstractions.Attributes;
using MessagePack;

namespace Catalog.DataAccess.Models;

/// <summary>
///     Model of product presented in catalog
/// </summary>
[BusTopic(Name = "product")]
[ConnectionAlias("catalog")]
public class Product : MongoRepoEntity
{
    [HashEntryKey] 
    [IgnoreMember]
    public string Code => Title.Replace(" ", "");

    /// <summary>
    ///     Title of product
    /// </summary>
    [Key(1)]
    public string Title { get; set; }

    /// <summary>
    ///     Description of product
    /// </summary>
    [Key(2)]
    public string Description { get; set; }

    /// <summary>
    ///     Url of image cover
    /// </summary>
    [Key(3)]
    public string ImageUrl { get; set; }

    /// <summary>
    ///     Price of product
    /// </summary>
    [Key(4)]
    public decimal Price { get; set; }

    /// <summary>
    ///     Categories where product is presented
    /// </summary>
    [Key(5)]
    public Guid[] CategoryId { get; set; }

    /// <summary>
    ///     Nutrition facts for 100g of product
    /// </summary>
    [Key(6)]
    public NutritionFact NutritionFact { get; set; }

    /// <summary>
    ///     Total weight of product
    /// </summary>
    [Key(7)]
    public double ServingSize { get; set; }

    /// <summary>
    ///     Ingredients product made of
    /// </summary>
    [Key(8)]
    public IngredientUsage[] Ingredients { get; set; }

    /// <summary>
    ///     Options available for product
    /// </summary>
    [Key(9)]
    public Guid[] OptionId { get; set; }
}