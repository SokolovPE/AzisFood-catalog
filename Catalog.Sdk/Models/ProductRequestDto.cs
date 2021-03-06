using System;
using System.Collections.Generic;

namespace Catalog.Sdk.Models;

/// <summary>
///     Request data transfer object for product
/// </summary>
public record ProductRequestDto : IRequestDto
{
    /// <summary>
    ///     Title of product
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    ///     Description of product
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     Url of image cover
    /// </summary>
    public string ImageUrl { get; set; }

    /// <summary>
    ///     Price of product
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    ///     Categories where product is presented
    /// </summary>
    public Guid[] CategoryId { get; set; }

    /// <summary>
    ///     Nutrition facts of this product
    /// </summary>
    public NutritionFactDto NutritionFact { get; set; }

    /// <summary>
    ///     Total weight of product
    /// </summary>
    public double ServingSize { get; set; }

    /// <summary>
    ///     Ingredients product made of
    /// </summary>
    public IEnumerable<IngredientUsageDto> Ingredients { get; set; }

    /// <summary>
    ///     Options available for product
    /// </summary>
    public Guid[] OptionId { get; set; }
}