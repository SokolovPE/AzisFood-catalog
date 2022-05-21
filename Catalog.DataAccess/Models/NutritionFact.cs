using AzisFood.DataEngine.Postgres.Models;
using MessagePack;
using Newtonsoft.Json;

namespace Catalog.DataAccess.Models;

/// <summary>
///     Product nutrition facts per 100g
/// </summary>
public class NutritionFact : PgRepoEntity<CatalogDbContext>
{
    /// <summary>
    ///     Hide original id from entity
    /// </summary>
    // [JsonIgnore]
    // [IgnoreMember]
    // [System.ComponentModel.DataAnnotations.Key]
    // internal new string Id { get; set; }

    // Energy in kJoules
    [Key(1)]
    public double Energy { get; set; }

    // Calories in kCal
    [Key(2)]
    public double Calories { get; set; }

    // Fat amount
    [Key(3)]
    public double TotalFat { get; set; }

    // Carbohydrates amount
    [Key(4)]
    public double Carbohydrates { get; set; }

    // Protein amount
    [Key(5)]
    public double Proteins { get; set; }

    // Sugar amount
    [Key(6)]
    public double TotalSugar { get; set; }
}