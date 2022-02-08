using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AzisFood.DataEngine.Abstractions.Interfaces;
using Catalog.Core.Services.Interfaces;
using Catalog.DataAccess.Models;
using Catalog.Sdk.Models;
using Microsoft.Extensions.Logging;

namespace Catalog.Core.Services.Implementations;

/// <summary>
///     Service to operate products
/// </summary>
public class ProductService : BaseService<Product, ProductDto, ProductRequestDto>, IProductService
{
    private readonly ICachedBaseRepository<Category> _categoryRepository;
    private readonly ICachedBaseRepository<Ingredient> _ingredientRepository;
    private readonly ICachedBaseRepository<Option> _optionRepository;
    private readonly IValidatorService<Product> _validator;

    /// <inheritdoc />
    public ProductService(ILogger<ProductService> logger,
        IMapper mapper,
        ICachedBaseRepository<Product> repository,
        IValidatorService<Product> validator, ICachedBaseRepository<Ingredient> ingredientRepository,
        ICachedBaseRepository<Category> categoryRepository, ICachedBaseRepository<Option> optionRepository)
        : base(logger, mapper, repository)
    {
        _validator = validator;
        _ingredientRepository = ingredientRepository;
        _categoryRepository = categoryRepository;
        _optionRepository = optionRepository;
    }


    /// <inheritdoc />
    public async Task<ProductDto[]> GetProductsInCategory(Guid categoryId, CancellationToken token = default)
    {
        try
        {
            var items =
                await Repository.GetHashAsync(product => product.CategoryId.Contains(categoryId), token);
            return Mapper.Map<ProductDto[]>(items);
        }
        catch (OperationCanceledException)
        {
            // Throw cancelled operation, do not catch
            throw;
        }
        catch (Exception e)
        {
            Logger.LogError(e,
                $"Exception during attempt to get {EntityName} filtered by category {categoryId} from catalog");
            throw;
        }
    }

    /// <inheritdoc />
    public override async Task<ProductDto> AddAsync(ProductRequestDto item, CancellationToken token = default)
    {
        try
        {
            var itemToInsert = Mapper.Map<Product>(item);
            var (validationResult, validationMessage) = await _validator.Validate(itemToInsert);
            if (!validationResult) throw new ValidationException(validationMessage);
            var insertedItem = await Repository.CreateAsync(itemToInsert, token);
            return Mapper.Map<ProductDto>(insertedItem);
        }
        catch (OperationCanceledException)
        {
            // Throw cancelled operation, do not catch
            throw;
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"Exception during attempt to insert record of {EntityName}");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteIngredients(IEnumerable<Guid> ingredientIds, CancellationToken token = default)
    {
        var products = await Repository.GetAsync(token);
        var productsToUpdate = products.Where(p =>
            p.Ingredients.Select(i => i.IngredientId).Intersect(ingredientIds.ToImmutableList()).Any());
        var toUpdate = productsToUpdate as Product[] ?? productsToUpdate.ToArray();
        try
        {
            foreach (var product in toUpdate)
            {
                product.Ingredients = product.Ingredients
                    .Where(p => !ingredientIds.Contains(p.IngredientId)).ToArray();
                await Repository.UpdateAsync(product.Id, product, token);
            }
        }
        catch (OperationCanceledException)
        {
            // Throw cancelled operation, do not catch
            throw;
        }
        catch (Exception e)
        {
            Logger.LogError(e,
                "Exception during attempt to remove deleted ingredient from products: " +
                $"{string.Join(',', toUpdate.Select(p => p.Id))}");
        }
    }

    /// <inheritdoc />
    public async Task SetCategories(Guid productId, IEnumerable<Guid> categoryIds,
        CancellationToken token = default)
    {
        var (existingCategories, notFoundCategories) =
            await ValidateCategories(categoryIds.ToArray(), token);
        var productItem = await GetEntityByIdAsync(productId, token);
        productItem.CategoryId = existingCategories;
        await Repository.UpdateAsync(productId, productItem, token);
        if (notFoundCategories.Length > 0)
            Logger.LogWarning(
                $"Attempt to set categories, which are not present on server: {string.Join(',', notFoundCategories)}, product: {productId}");
    }

    /// <inheritdoc />
    public async Task AssignCategories(Guid productId, IEnumerable<Guid> categoryIds,
        CancellationToken token = default)
    {
        var (existingCategories, notFoundCategories) =
            await ValidateCategories(categoryIds.ToArray(), token);
        var productItem = await GetEntityByIdAsync(productId, token);
        productItem.CategoryId = productItem.CategoryId.Concat(existingCategories).ToArray();
        await Repository.UpdateAsync(productId, productItem, token);
        if (notFoundCategories.Length > 0)
            Logger.LogWarning(
                $"Attempt to assign categories, which are not present on server: {string.Join(',', notFoundCategories)}, product: {productId}");
    }

    /// <inheritdoc />
    public async Task RetainCategories(Guid productId, IEnumerable<Guid> categoryIds,
        CancellationToken token = default)
    {
        var productItem = await GetEntityByIdAsync(productId, token);
        productItem.CategoryId = productItem.CategoryId.Where(cat => !categoryIds.Contains(cat)).ToArray();
        await Repository.UpdateAsync(productId, productItem, token);
    }

    /// <inheritdoc />
    public async Task SetIngredients(Guid productId, IEnumerable<IngredientUsageDto> ingredientUsages,
        CancellationToken token = default)
    {
        var (existingCategories, notFoundCategories) =
            await ValidateIngredients(ingredientUsages, token);
        var productItem = await GetEntityByIdAsync(productId, token);
        productItem.Ingredients = Mapper.Map<IEnumerable<IngredientUsage>>(existingCategories).ToArray();
        await Repository.UpdateAsync(productId, productItem, token);
        if (notFoundCategories.Length > 0)
            Logger.LogWarning(
                $"Attempt to set usage of ingredients, which are not present on server: {string.Join(',', notFoundCategories.Select(x => x.ToString()))}, product: {productId}");
    }

    /// <inheritdoc />
    public async Task AssignIngredients(Guid productId, IEnumerable<IngredientUsageDto> ingredientUsages,
        CancellationToken token = default)
    {
        var (existingCategories, notFoundCategories) =
            await ValidateIngredients(ingredientUsages, token);
        var productItem = await GetEntityByIdAsync(productId, token);
        var usages = Mapper.Map<IEnumerable<IngredientUsage>>(existingCategories).ToArray();
        productItem.Ingredients = productItem.Ingredients.Concat(usages).ToArray();
        await Repository.UpdateAsync(productId, productItem, token);
        if (notFoundCategories.Length > 0)
            Logger.LogWarning(
                $"Attempt to assign usage of ingredients, which are not present on server: {string.Join(',', notFoundCategories.Select(x => x.ToString()))}, product: {productId}");
    }

    /// <inheritdoc />
    public async Task RetainIngredients(Guid productId, IEnumerable<Guid> ingredientIds,
        CancellationToken token = default)
    {
        var productItem = await GetEntityByIdAsync(productId, token);
        productItem.Ingredients = productItem.Ingredients.Where(cat => !ingredientIds.Contains(cat.IngredientId))
            .ToArray();
        await Repository.UpdateAsync(productId, productItem, token);
    }

    /// <inheritdoc />
    public async Task SetOptions(Guid productId, IEnumerable<Guid> optionIds, CancellationToken token = default)
    {
        var (existingOptions, notFoundOptions) =
            await ValidateOptions(optionIds.ToArray(), token);
        var productItem = await GetEntityByIdAsync(productId, token);
        productItem.OptionId = existingOptions;
        await Repository.UpdateAsync(productId, productItem, token);
        if (notFoundOptions.Length > 0)
            Logger.LogWarning(
                $"Attempt to set options, which are not present on server: {string.Join(',', notFoundOptions)}, product: {productId}");
    }

    /// <inheritdoc />
    public async Task AssignOptions(Guid productId, IEnumerable<Guid> optionIds, CancellationToken token = default)
    {
        var (existingOptions, notFoundOptions) =
            await ValidateOptions(optionIds.ToArray(), token);
        var productItem = await GetEntityByIdAsync(productId, token);
        productItem.OptionId = productItem.CategoryId.Concat(existingOptions).ToArray();
        await Repository.UpdateAsync(productId, productItem, token);
        if (notFoundOptions.Length > 0)
            Logger.LogWarning(
                $"Attempt to assign options, which are not present on server: {string.Join(',', notFoundOptions)}, product: {productId}");
    }

    /// <inheritdoc />
    public async Task RetainOptions(Guid productId, IEnumerable<Guid> optionIds, CancellationToken token = default)
    {
        var productItem = await GetEntityByIdAsync(productId, token);
        productItem.CategoryId = productItem.OptionId.Where(cat => !optionIds.Contains(cat)).ToArray();
        await Repository.UpdateAsync(productId, productItem, token);
    }

    /// <summary>
    ///     Split categories into two arrays, which exists at server and which not
    /// </summary>
    /// <param name="inputCategories">Categories to check</param>
    /// <param name="token">Token to cancel operation</param>
    private async Task<(Guid[] found, Guid[] notFound)> ValidateCategories(Guid[] inputCategories,
        CancellationToken token = default)
    {
        var existingCategories =
            (await _categoryRepository.GetHashAsync(c => inputCategories.Contains(c.Id), token))
            .Select(x => x.Id)
            .ToArray();
        var notFoundCategories = inputCategories.Except(existingCategories).ToArray();
        return (existingCategories, notFoundCategories);
    }

    /// <summary>
    ///     Split ingredients into two arrays, which exists at server and which not
    /// </summary>
    /// <param name="inputIngredients">Ingredients to check</param>
    /// <param name="token">Token to cancel operation</param>
    private async Task<(IngredientUsageDto[] found, IngredientUsageDto[] notFound)> ValidateIngredients(
        IEnumerable<IngredientUsageDto> inputIngredients,
        CancellationToken token = default)
    {
        var input = inputIngredients.ToArray();
        var inputIngredientIds = input.Select(x => x.IngredientId).ToArray();
        var existingIngredientIds =
            (await _ingredientRepository.GetHashAsync(i => inputIngredientIds.Contains(i.Id), token))
            .Select(x => x.Id)
            .ToArray();
        var notFoundIngredientIds = inputIngredientIds.Except(existingIngredientIds).ToArray();

        var existingIngredients =
            input.Where(x => existingIngredientIds.Contains(x.IngredientId)).ToArray();
        var notFoundIngredients =
            input.Where(x => notFoundIngredientIds.Contains(x.IngredientId)).ToArray();

        return (existingIngredients, notFoundIngredients);
    }

    /// <summary>
    ///     Split options into two arrays, which exists at server and which not
    /// </summary>
    /// <param name="inputOptions">Options to check</param>
    /// <param name="token">Token to cancel operation</param>
    private async Task<(Guid[] found, Guid[] notFound)> ValidateOptions(Guid[] inputOptions,
        CancellationToken token = default)
    {
        var existingOptions =
            (await _optionRepository.GetHashAsync(c => inputOptions.Contains(c.Id), token))
            .Select(x => x.Id)
            .ToArray();
        var notFoundOptions = inputOptions.Except(existingOptions).ToArray();
        return (existingOptions, notFoundOptions);
    }
}