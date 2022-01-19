using System.Collections.Generic;
using System.Threading.Tasks;
using AzisFood.DataEngine.Interfaces;
using Catalog.DataAccess.Models;
using HotChocolate;
using HotChocolate.Data;

namespace Catalog.GraphQL;

/// <summary>
/// GraphQL demo query
/// </summary>
public class Query
{
    /// <summary>
    /// Get list of products
    /// </summary>
    [UseFiltering]
    [UseSorting]
    public Task<IEnumerable<Product>> GetProduct([Service] ICachedBaseRepository<Product> repository) =>
        repository.GetHashAsync();

    /// <summary>
    /// Get list of categories
    /// </summary>
    [UseFiltering]
    [UseSorting]
    public Task<IEnumerable<Category>> GetCategory([Service] ICachedBaseRepository<Category> repository) =>
        repository.GetHashAsync();

    /// <summary>
    /// Get list of ingredients
    /// </summary>
    [UseFiltering]
    [UseSorting]
    public Task<IEnumerable<Ingredient>> GetIngredient([Service] ICachedBaseRepository<Ingredient> repository) =>
        repository.GetHashAsync();

    /// <summary>
    /// Get list of options
    /// </summary>
    [UseFiltering]
    [UseSorting]
    public Task<IEnumerable<Option>> GetOption([Service] ICachedBaseRepository<Option> repository) =>
        repository.GetHashAsync();
}