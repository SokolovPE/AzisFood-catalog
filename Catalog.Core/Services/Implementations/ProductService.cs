using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AzisFood.DataEngine.Interfaces;
using Catalog.Core.Services.Interfaces;
using Catalog.DataAccess.Models;
using Catalog.Sdk.Models;
using Microsoft.Extensions.Logging;

namespace Catalog.Core.Services.Implementations
{
    /// <summary>
    /// Service to operate products
    /// </summary>
    public class ProductService : BaseService<Product, ProductDto, ProductRequestDto>, IProductService
    {
        private readonly IValidatorService<Product> _validator;
        private readonly ICachedBaseRepository<Ingredient> _ingredientRepository;
        private readonly ICachedBaseRepository<Category> _categoryRepository;

        /// <inheritdoc />
        public ProductService(ILogger<ProductService> logger,
            IMapper mapper,
            ICachedBaseRepository<Product> repository,
            IValidatorService<Product> validator, ICachedBaseRepository<Ingredient> ingredientRepository,
            ICachedBaseRepository<Category> categoryRepository)
            : base(logger, mapper, repository)
        {
            _validator = validator;
            _ingredientRepository = ingredientRepository;
            _categoryRepository = categoryRepository;
        }

        /// <inheritdoc />
        public override async Task<ProductDto> AddAsync(ProductRequestDto item, CancellationToken token = default)
        {
            try
            {
                var itemToInsert = Mapper.Map<Product>(item);
                var (validationResult, validationMessage) = await _validator.Validate(itemToInsert);
                if (!validationResult)
                {
                    throw new ValidationException(validationMessage);
                }
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
        public async Task DeleteIngredients(IEnumerable<string> ingredientIds, CancellationToken token = default)
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
        public async Task SetCategories(string productId, IEnumerable<string> categoryIds,
            CancellationToken token = default)
        {
            //TODO: filter existing only
            var productItem = await GetEntityByIdAsync(productId, token);
            productItem.CategoryId = categoryIds.ToArray();
            await Repository.UpdateAsync(productId, productItem, token);
        }

        /// <inheritdoc />
        public async Task AssignCategories(string productId, IEnumerable<string> categoryIds,
            CancellationToken token = default)
        {
            //TODO: filter existing only
            var productItem = await GetEntityByIdAsync(productId, token);
            productItem.CategoryId = productItem.CategoryId.Concat(categoryIds).ToArray();
            await Repository.UpdateAsync(productId, productItem, token);
        }

        /// <inheritdoc />
        public async Task RetainCategories(string productId, IEnumerable<string> categoryIds,
            CancellationToken token = default)
        {
            var productItem = await GetEntityByIdAsync(productId, token);
            productItem.CategoryId = productItem.CategoryId.Where(cat => !categoryIds.Contains(cat)).ToArray();
            await Repository.UpdateAsync(productId, productItem, token);
        }

        /// <inheritdoc />
        public async Task SetIngredients(string productId, IEnumerable<IngredientUsageDto> ingredientUsages,
            CancellationToken token = default)
        {
            //TODO: filter existing only
            var productItem = await GetEntityByIdAsync(productId, token);
            productItem.Ingredients = Mapper.Map<IEnumerable<IngredientUsage>>(ingredientUsages).ToArray();
            await Repository.UpdateAsync(productId, productItem, token);
        }

        /// <inheritdoc />
        public async Task AssignIngredients(string productId, IEnumerable<IngredientUsageDto> ingredientUsages,
            CancellationToken token = default)
        {
            //TODO: filter existing only
            var productItem = await GetEntityByIdAsync(productId, token);
            var usages = Mapper.Map<IEnumerable<IngredientUsage>>(ingredientUsages).ToArray();
            productItem.Ingredients = productItem.Ingredients.Concat(usages).ToArray();
            await Repository.UpdateAsync(productId, productItem, token);
        }

        /// <inheritdoc />
        public async Task RetainIngredients(string productId, IEnumerable<string> ingredientIds,
            CancellationToken token = default)
        {
            var productItem = await GetEntityByIdAsync(productId, token);
            productItem.Ingredients = productItem.Ingredients.Where(cat => !ingredientIds.Contains(cat.IngredientId))
                .ToArray();
            await Repository.UpdateAsync(productId, productItem, token);
        }
    }
}