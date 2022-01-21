using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Catalog.DataAccess.Models;
using Catalog.Sdk.Models;

namespace Catalog.Core.Services.Interfaces
{
    /// <inheritdoc />
    public interface IProductService : IService<Product, ProductDto, ProductRequestDto>
    {
        /// <summary>
        /// Get products in provided category
        /// </summary>
        /// <param name="categoryId">Id of category to search for</param>
        /// <param name="token">Token for operation cancel</param>
        /// <returns>List of products in provided category</returns>
        Task<ProductDto[]> GetProductsInCategory(Guid categoryId, CancellationToken token = default);
        
        /// <summary>
        /// Delete listed ingredients from all products
        /// </summary>
        /// <param name="ingredientIds">Identifiers of ingredient to be removed</param>
        /// <param name="token">Token for operation cancel</param>
        Task DeleteIngredients(IEnumerable<Guid> ingredientIds, CancellationToken token = default);

        /// <summary>
        /// Set categories to product, will replace existing ones
        /// </summary>
        /// <param name="productId">ID of product to set to</param>
        /// <param name="categoryIds">Collection of category IDs to be set</param>
        /// <param name="token">Token for operation cancel</param>
        Task SetCategories(Guid productId, IEnumerable<Guid> categoryIds, CancellationToken token = default);
        
        /// <summary>
        /// Assign additional categories to product
        /// </summary>
        /// <param name="productId">ID of product to assign to</param>
        /// <param name="categoryIds">Collection of category IDs to be assigned</param>
        /// <param name="token">Token for operation cancel</param>
        Task AssignCategories(Guid productId, IEnumerable<Guid> categoryIds, CancellationToken token = default);

        /// <summary>
        /// Retain categories from product
        /// </summary>
        /// <param name="productId">ID of product to retain from</param>
        /// <param name="categoryIds">Collection of category IDs to be retained</param>
        /// <param name="token">Token for operation cancel</param>
        Task RetainCategories(Guid productId, IEnumerable<Guid> categoryIds, CancellationToken token = default);

        /// <summary>
        /// Set ingredients to product, will replace existing ones
        /// </summary>
        /// <param name="productId">ID of product to set to</param>
        /// <param name="ingredientUsages">Collection of ingredient usages to be set</param>
        /// <param name="token">Token for operation cancel</param>
        Task SetIngredients(Guid productId, IEnumerable<IngredientUsageDto> ingredientUsages,
            CancellationToken token = default);

        /// <summary>
        /// Assign additional ingredients to product
        /// </summary>
        /// <param name="productId">ID of product to assign to</param>
        /// <param name="ingredientUsages">Array of ingredient usages to be assigned</param>
        /// <param name="token">Token for operation cancel</param>
        Task AssignIngredients(Guid productId, IEnumerable<IngredientUsageDto> ingredientUsages,
            CancellationToken token = default);

        /// <summary>
        /// Retain ingredients from product
        /// </summary>
        /// <param name="productId">ID of product to retain from</param>
        /// <param name="ingredientIds">Collection of ingredient IDs to be retained</param>
        /// <param name="token">Token for operation cancel</param>
        Task RetainIngredients(Guid productId, IEnumerable<Guid> ingredientIds, CancellationToken token = default);
        
        /// <summary>
        /// Set available options to product, will replace existing ones
        /// </summary>
        /// <param name="productId">ID of product to set to</param>
        /// <param name="optionIds">Collection of option IDs to be set</param>
        /// <param name="token">Token for operation cancel</param>
        Task SetOptions(Guid productId, IEnumerable<Guid> optionIds, CancellationToken token = default);
        
        /// <summary>
        /// Assign available options to product
        /// </summary>
        /// <param name="productId">ID of product to assign to</param>
        /// <param name="optionIds">Collection of option IDs to be assigned</param>
        /// <param name="token">Token for operation cancel</param>
        Task AssignOptions(Guid productId, IEnumerable<Guid> optionIds, CancellationToken token = default);

        /// <summary>
        /// Retain available options from product
        /// </summary>
        /// <param name="productId">ID of option to retain from</param>
        /// <param name="optionIds">Collection of option IDs to be retained</param>
        /// <param name="token">Token for operation cancel</param>
        Task RetainOptions(Guid productId, IEnumerable<Guid> optionIds, CancellationToken token = default);
    }
}