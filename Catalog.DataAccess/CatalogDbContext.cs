using AzisFood.DataEngine.Core.Attributes;
using Microsoft.EntityFrameworkCore;

namespace Catalog.DataAccess;

[ConnectionAlias("catalog")]
public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Models.Category> Categories { get; set; } = null!;
    public DbSet<Models.Ingredient> Ingredients { get; set; } = null!;
    public DbSet<Models.IngredientUsage> IngredientUsages { get; set; } = null!;
    public DbSet<Models.NutritionFact> NutritionFacts { get; set; } = null!;
    public DbSet<Models.Option> Options { get; set; } = null!;
    public DbSet<Models.Product> Products { get; set; } = null!;
}