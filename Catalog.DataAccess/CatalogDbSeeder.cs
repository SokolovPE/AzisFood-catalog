using System;
using System.Linq;
using Catalog.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Catalog.DataAccess;

/// <summary>
///     This seeder fills catalog database with initial values
///     Can be disabled in application settings
/// </summary>
public class CatalogDbSeeder
{
    /// <summary>
    ///     Catalog context
    /// </summary>
    private readonly CatalogDbContext _context;

    /// <summary>
    ///     Logger
    /// </summary>
    private readonly ILogger<CatalogDbSeeder> _logger;

    public CatalogDbSeeder(IDbContextFactory<CatalogDbContext> contextFactory, ILogger<CatalogDbSeeder> logger)
    {
        _logger = logger;
        _context = contextFactory.CreateDbContext();
    }

    /// <summary>
    ///     Common seed method
    /// </summary>
    public void Seed()
    {
        SeedCategories();
        SeedIngredients();
        SeedOptions();
        SeedProducts();
    }

    /// <summary>
    ///     Fill categories if there's none
    /// </summary>
    private void SeedCategories()
    {
        if (_context.Categories.Any())
            return;

        var categorySeed = new[]
        {
            new Category
            {
                Id = Guid.Parse("9a1aff75-a5a8-4259-ac77-02832276f2cb"),
                Title = "Burgers",
                Order = 0
            },
            new Category
            {
                Id = Guid.Parse("a7df8c1d-5d35-41bf-9894-5c2e413227ba"),
                Title = "Featured",
                Order = 1
            },
            new Category
            {
                Id = Guid.Parse("e807a75e-fae8-4163-86a0-439d0d0c320d"),
                Title = "Combos",
                Order = 2
            },
            new Category
            {
                Id = Guid.Parse("4e8d6b29-df11-4b85-824c-940e6b0b1b51"),
                Title = "Shawarma",
                Order = 3
            },
            new Category
            {
                Id = Guid.Parse("87aa0819-615f-4d04-b7c7-e3b907de0cb0"),
                Title = "Pizza",
                Order = 4
            },
            new Category
            {
                Id = Guid.Parse("8e6ddcec-c50c-4544-be48-66ce5480a38a"),
                Title = "Snacks",
                Order = 5
            },
            new Category
            {
                Id = Guid.Parse("ab234970-e691-4bc5-bbb6-b4440d7c52c3"),
                Title = "Salads",
                Order = 6
            },
            new Category
            {
                Id = Guid.Parse("11a7aa91-21fa-4d46-b276-55f995d323af"),
                Title = "Drinks",
                Order = 7
            },
            new Category
            {
                Id = Guid.Parse("b7aa57a7-4c70-4576-985d-de340016ec0f"),
                Title = "Sweets",
                Order = 8
            },
            new Category
            {
                Id = Guid.Parse("feab161f-170a-41de-84e6-584212646c12"),
                Title = "Sauces",
                Order = 9
            }
        };

        _context.Categories.AddRange(categorySeed);
        var changes = _context.SaveChanges();
        if (changes != categorySeed.Length)
            _logger.LogWarning(
                $"[Categories] Seed length {categorySeed.Length} is not equal to added row count: {changes}");
    }

    /// <summary>
    ///     Fill ingredients if there's none
    /// </summary>
    private void SeedIngredients()
    {
        if (_context.Ingredients.Any())
            return;

        var ingredientSeed = new[]
        {
            new Ingredient
            {
                Id = Guid.Parse("a8d0a313-da81-426d-9df4-98fff2a2efaf"),
                Title = "Bacon",
                MeasureUnitId = "56e47c09-a1d5-4052-ad8a-96ef26680846"
            },
            new Ingredient
            {
                Id = Guid.Parse("05ed90ba-3b98-47aa-8ef0-50779cc04ae1"),
                Title = "Tomato",
                MeasureUnitId = "56e47c09-a1d5-4052-ad8a-96ef26680846"
            },
            new Ingredient
            {
                Id = Guid.Parse("cd7232ed-fa4d-4d89-a8d9-44f038bffb45"),
                Title = "Cheddar",
                MeasureUnitId = "ca602653-372e-4ec0-8d4f-e579220b05c2"
            },
            new Ingredient
            {
                Id = Guid.Parse("6f589adc-7b56-42bf-9bb5-6918e04fcff9"),
                Title = "Salad",
                MeasureUnitId = "56e47c09-a1d5-4052-ad8a-96ef26680846"
            },
            new Ingredient
            {
                Id = Guid.Parse("18c7f263-3bde-424f-892c-154bb78f3f57"),
                Title = "Beef cutlet",
                MeasureUnitId = "ca602653-372e-4ec0-8d4f-e579220b05c2"
            },
            new Ingredient
            {
                Id = Guid.Parse("dd33a3c9-c95c-40c2-8154-2cf1361876cf"),
                Title = "Chicken fillet",
                MeasureUnitId = "ca602653-372e-4ec0-8d4f-e579220b05c2"
            },
            new Ingredient
            {
                Id = Guid.Parse("d7c04943-3e2c-409c-8ef5-f05b7645fba3"),
                Title = "Ketchup",
                MeasureUnitId = "56e47c09-a1d5-4052-ad8a-96ef26680846"
            },
            new Ingredient
            {
                Id = Guid.Parse("ec2da6b8-2a2b-4584-936d-2767ff6cbb41"),
                Title = "Onion",
                MeasureUnitId = "56e47c09-a1d5-4052-ad8a-96ef26680846"
            },
            new Ingredient
            {
                Id = Guid.Parse("6dbaf0b1-e1ca-4524-8c21-967114da0ebc"),
                Title = "Cucumber",
                MeasureUnitId = "56e47c09-a1d5-4052-ad8a-96ef26680846"
            },
            new Ingredient
            {
                Id = Guid.Parse("50457539-5e89-4b88-8142-64c4f974cea6"),
                Title = "Bun",
                MeasureUnitId = "ca602653-372e-4ec0-8d4f-e579220b05c2"
            },
            new Ingredient
            {
                Id = Guid.Parse("0dd5fcd6-a084-4758-b153-49b500e9c600"),
                Title = "Mayonnaise",
                MeasureUnitId = "56e47c09-a1d5-4052-ad8a-96ef26680846"
            },
            new Ingredient
            {
                Id = Guid.Parse("13835808-4367-4efa-a912-f0d35b0db84b"),
                Title = "Gauda",
                MeasureUnitId = "ca602653-372e-4ec0-8d4f-e579220b05c2"
            }
        };

        _context.Ingredients.AddRange(ingredientSeed);
        var changes = _context.SaveChanges();
        if (changes != ingredientSeed.Length)
            _logger.LogWarning(
                $"[Ingredients] Seed length {ingredientSeed.Length} is not equal to added row count: {changes}");
    }

    /// <summary>
    ///     Fill options if there's none
    /// </summary>
    private void SeedOptions()
    {
        if (_context.Options.Any())
            return;

        var optionSeed = new[]
        {
            new Option
            {
                Id = Guid.Parse("5a45ccc3-76d3-4355-8afc-44046d69bcce"),
                Title = "Firm cheese",
                ImageUrl = "www.dummy.com",
                Price = 1.99m
            },
            new Option
            {
                Id = Guid.Parse("0e05bafe-ec03-4efc-9251-9b4c6584edcd"),
                Title = "Bacon",
                ImageUrl = "www.dummy.com",
                Price = 2.99m
            },
            new Option
            {
                Id = Guid.Parse("2fa9789f-4615-4c8e-ae7f-46a17dc6145e"),
                Title = "Cheese",
                ImageUrl = "www.dummy.com",
                Price = 0.99m
            },
            new Option
            {
                Id = Guid.Parse("f4dcf3f1-55d0-4ddd-899f-0d88556a7212"),
                Title = "Cutlet",
                ImageUrl = "www.dummy.com",
                Price = 3.99m
            }
        };

        _context.Options.AddRange(optionSeed);
        var changes = _context.SaveChanges();
        if (changes != optionSeed.Length)
            _logger.LogWarning($"[Options] Seed length {optionSeed.Length} is not equal to added row count: {changes}");
    }

    /// <summary>
    ///     Fill products if there's none
    /// </summary>
    private void SeedProducts()
    {
        if (_context.Products.Any())
            return;

        var productSeed = new[]
        {
            new Product
            {
                Id = Guid.Parse("c69c89e6-81ea-4d4a-a7fb-4618150682b5"),
                Title = "Shmerburger",
                Description = "This is the greatest burger you ever ate!",
                ImageUrl = "/public/goods/burgers/baconMcBeef.png",
                Price = 5.99m,
                CategoryId = new[]
                {
                    Guid.Parse("9a1aff75-a5a8-4259-ac77-02832276f2cb")
                },
                NutritionFact = new NutritionFact
                {
                    // Id = Guid.NewGuid(),
                    Energy = 120.77,
                    Calories = 415.2,
                    TotalFat = 200.18,
                    Carbohydrates = 78.29,
                    Proteins = 6.89,
                    TotalSugar = 150
                },
                ServingSize = 250,
                Ingredients = new[]
                {
                    new IngredientUsage
                    {
                        // Id = Guid.NewGuid(),
                        IngredientId = Guid.Parse("50457539-5e89-4b88-8142-64c4f974cea6"),
                        Amount = 1,
                        Toggleable = false
                    },
                    new IngredientUsage
                    {
                        // Id = Guid.NewGuid(),
                        IngredientId = Guid.Parse("6f589adc-7b56-42bf-9bb5-6918e04fcff9"),
                        Amount = 50,
                        Toggleable = false
                    },
                    new IngredientUsage
                    {
                        // Id = Guid.NewGuid(),
                        IngredientId = Guid.Parse("ec2da6b8-2a2b-4584-936d-2767ff6cbb41"),
                        Amount = 25,
                        Toggleable = false
                    },
                    new IngredientUsage
                    {
                        // Id = Guid.NewGuid(),
                        IngredientId = Guid.Parse("6dbaf0b1-e1ca-4524-8c21-967114da0ebc"),
                        Amount = 20,
                        Toggleable = false
                    },
                    new IngredientUsage
                    {
                        // Id = Guid.NewGuid(),
                        IngredientId = Guid.Parse("05ed90ba-3b98-47aa-8ef0-50779cc04ae1"),
                        Amount = 15,
                        Toggleable = false
                    },
                    new IngredientUsage
                    {
                        // Id = Guid.NewGuid(),
                        IngredientId = Guid.Parse("18c7f263-3bde-424f-892c-154bb78f3f57"),
                        Amount = 1,
                        Toggleable = false
                    },
                    new IngredientUsage
                    {
                        // Id = Guid.NewGuid(),
                        IngredientId = Guid.Parse("13835808-4367-4efa-a912-f0d35b0db84b"),
                        Amount = 1,
                        Toggleable = false
                    },
                    new IngredientUsage
                    {
                        // Id = Guid.NewGuid(),
                        IngredientId = Guid.Parse("0dd5fcd6-a084-4758-b153-49b500e9c600"),
                        Amount = 20,
                        Toggleable = false
                    },
                    new IngredientUsage
                    {
                        // Id = Guid.NewGuid(),
                        IngredientId = Guid.Parse("d7c04943-3e2c-409c-8ef5-f05b7645fba3"),
                        Amount = 15,
                        Toggleable = false
                    }
                }
            },
            new Product
            {
                Id = Guid.Parse("e2aeb8d7-9385-48e1-be79-b3fd82ac0870"),
                Title = "Chickburger Cheese",
                Description = "Super duper burger",
                ImageUrl = "/public/goods/burgers/mcChickCheese.png",
                Price = 17.59m,
                CategoryId = new[]
                {
                    Guid.Parse("9a1aff75-a5a8-4259-ac77-02832276f2cb")
                },
                NutritionFact = new NutritionFact
                {
                    // Id = Guid.NewGuid(),
                    Energy = 1,
                    Calories = 2,
                    TotalFat = 3,
                    Carbohydrates = 4,
                    Proteins = 5,
                    TotalSugar = 6
                },
                ServingSize = 1
            },
            new Product
            {
                Id = Guid.Parse("291949d4-a539-4e4e-ab20-bd361769cc1d"),
                Title = "Megaburger",
                Description = "Super duper burger",
                ImageUrl = "/public/goods/burgers/mcBeefFat.png",
                Price = 15.99m,
                CategoryId = new[]
                {
                    Guid.Parse("9a1aff75-a5a8-4259-ac77-02832276f2cb")
                },
                NutritionFact = new NutritionFact
                {
                    // Id = Guid.NewGuid(),
                    Energy = 1,
                    Calories = 2,
                    TotalFat = 3,
                    Carbohydrates = 4,
                    Proteins = 5,
                    TotalSugar = 6
                },
                ServingSize = 1
            },
            new Product
            {
                Id = Guid.Parse("7aeaa486-0038-4237-a85f-c4fe888b95e8"),
                Title = "Chickburger",
                Description = "Super duper burger",
                ImageUrl = "/public/goods/burgers/mcChick.png",
                Price = 17.59m,
                CategoryId = new[]
                {
                    Guid.Parse("9a1aff75-a5a8-4259-ac77-02832276f2cb")
                },
                NutritionFact = new NutritionFact
                {
                    // Id = Guid.NewGuid(),
                    Energy = 1,
                    Calories = 2,
                    TotalFat = 3,
                    Carbohydrates = 4,
                    Proteins = 5,
                    TotalSugar = 6
                },
                ServingSize = 1
            },
            new Product
            {
                Id = Guid.Parse("bd3254d5-3f93-46ec-82b9-c8b721a3b01a"),
                Title = "New product",
                Description = "Something delicious",
                ImageUrl = "/public/goods/burgers/mcBeef.png",
                Price = 12.75m,
                CategoryId = new[]
                {
                    Guid.Parse("9a1aff75-a5a8-4259-ac77-02832276f2cb")
                },
                NutritionFact = new NutritionFact
                {
                    // Id = Guid.NewGuid(),
                    Energy = 10,
                    Calories = 10,
                    TotalFat = 10,
                    Carbohydrates = 10,
                    Proteins = 10,
                    TotalSugar = 10
                },
                ServingSize = 100,
                Ingredients = new[]
                {
                    new IngredientUsage
                    {
                        // Id = Guid.NewGuid(),
                        IngredientId = Guid.Parse("6f589adc-7b56-42bf-9bb5-6918e04fcff9"),
                        Amount = 99,
                        Toggleable = true
                    },
                    new IngredientUsage
                    {
                        // Id = Guid.NewGuid(),
                        IngredientId = Guid.Parse("6dbaf0b1-e1ca-4524-8c21-967114da0ebc"),
                        Amount = 99,
                        Toggleable = false
                    }
                },
                OptionId = new[]
                {
                    Guid.Parse("0e05bafe-ec03-4efc-9251-9b4c6584edcd"),
                    Guid.Parse("2fa9789f-4615-4c8e-ae7f-46a17dc6145e"),
                    Guid.Parse("f4dcf3f1-55d0-4ddd-899f-0d88556a7212"),
                    Guid.Parse("5a45ccc3-76d3-4355-8afc-44046d69bcce")
                }
            },
            new Product
            {
                Id = Guid.Parse("6a12bfb6-6137-40d8-8c1b-3b795b912584"),
                Title = "Chickburger BBQ",
                Description = "Super duper burger",
                ImageUrl = "/public/goods/burgers/mcChickBBQ.png",
                Price = 17.59m,
                CategoryId = new[]
                {
                    Guid.Parse("9a1aff75-a5a8-4259-ac77-02832276f2cb")
                },
                NutritionFact = new NutritionFact
                {
                    // Id = Guid.NewGuid(),
                    Energy = 1,
                    Calories = 2,
                    TotalFat = 3,
                    Carbohydrates = 4,
                    Proteins = 5,
                    TotalSugar = 6
                },
                ServingSize = 1
            },
            new Product
            {
                Id = Guid.Parse("d28121ee-c1db-40a7-b53c-7988d7be00c9"),
                Title = "Chickburger Bastard",
                Description = "Super duper burger",
                ImageUrl = "/public/goods/burgers/mcChickBastard.png",
                Price = 17.59m,
                CategoryId = new[]
                {
                    Guid.Parse("9a1aff75-a5a8-4259-ac77-02832276f2cb")
                },
                NutritionFact = new NutritionFact
                {
                    // Id = Guid.NewGuid(),
                    Energy = 1,
                    Calories = 2,
                    TotalFat = 3,
                    Carbohydrates = 4,
                    Proteins = 5,
                    TotalSugar = 6
                },
                ServingSize = 1
            },
            new Product
            {
                Id = Guid.Parse("e9cda924-a087-467c-b74a-023011ddd2c0"),
                Title = "Kingburger",
                Description = "Super duper burger",
                ImageUrl = "/public/goods/burgers/royalMcBeef.png",
                Price = 17.59m,
                CategoryId = new[]
                {
                    Guid.Parse("9a1aff75-a5a8-4259-ac77-02832276f2cb")
                },
                NutritionFact = new NutritionFact
                {
                    // Id = Guid.NewGuid(),
                    Energy = 1,
                    Calories = 2,
                    TotalFat = 3,
                    Carbohydrates = 4,
                    Proteins = 5,
                    TotalSugar = 6
                },
                ServingSize = 1
            }
        };

        _context.Products.AddRange(productSeed);
        _context.SaveChanges();
    }
}