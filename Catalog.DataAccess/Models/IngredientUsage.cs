using System;
using AzisFood.DataEngine.Postgres.Models;
using MessagePack;

namespace Catalog.DataAccess.Models;

/// <summary>
///     Usage of ingredient in product
/// </summary>
[MessagePackObject]
public class IngredientUsage : PgRepoEntity<CatalogDbContext>
{
    /// <summary>
    ///     Identifier of used ingredient
    /// </summary>
    [Key(1)]
    public Guid IngredientId { get; set; }

    /// <summary>
    ///     Amount of used ingredient
    /// </summary>
    [Key(2)]
    public decimal Amount { get; set; }

    /// <summary>
    ///     Marker can ingredient be removed or not
    /// </summary>
    [Key(3)]
    public bool Toggleable { get; set; }
}