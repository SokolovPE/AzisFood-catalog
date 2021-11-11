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
        /// Delete listed ingredients from all products
        /// </summary>
        /// <param name="ingredientIds">Identifiers of ingredient to be removed</param>
        /// <param name="token">Token for operation cancel</param>
        Task DeleteIngredients(IEnumerable<string> ingredientIds, CancellationToken token = default);

        /// <summary>
        /// Set categories to product, will replace existing ones
        /// </summary>
        /// <param name="productId">ID of product to set to</param>
        /// <param name="categoryIds">Collection of category IDs to be set</param>
        /// <param name="token">Token for operation cancel</param>
        Task SetCategories(string productId, IEnumerable<string> categoryIds, CancellationToken token = default);
        
        /// <summary>
        /// Assign additional categories to product
        /// </summary>
        /// <param name="productId">ID of product to assign to</param>
        /// <param name="categoryIds">Collection of category IDs to be assigned</param>
        /// <param name="token">Token for operation cancel</param>
        Task AssignCategories(string productId, IEnumerable<string> categoryIds, CancellationToken token = default);

        /// <summary>
        /// Retain categories from product
        /// </summary>
        /// <param name="productId">ID of product to retain from</param>
        /// <param name="categoryIds">Collection of category IDs to be retained</param>
        /// <param name="token">Token for operation cancel</param>
        Task RetainCategories(string productId, IEnumerable<string> categoryIds, CancellationToken token = default);

        /// <summary>
        /// Set ingredients to product, will replace existing ones
        /// </summary>
        /// <param name="productId">ID of product to set to</param>
        /// <param name="ingredientUsages">Collection of ingredient usages to be set</param>
        /// <param name="token">Token for operation cancel</param>
        Task SetIngredients(string productId, IEnumerable<IngredientUsageDto> ingredientUsages,
            CancellationToken token = default);

        /// <summary>
        /// Assign additional ingredients to product
        /// </summary>
        /// <param name="productId">ID of product to assign to</param>
        /// <param name="ingredientUsages">Array of ingredient usages to be assigned</param>
        /// <param name="token">Token for operation cancel</param>
        Task AssignIngredients(string productId, IEnumerable<IngredientUsageDto> ingredientUsages,
            CancellationToken token = default);

        /// <summary>
        /// Retain ingredients from product
        /// </summary>
        /// <param name="productId">ID of product to retain from</param>
        /// <param name="ingredientIds">Collection of ingredient IDs to be retained</param>
        /// <param name="token">Token for operation cancel</param>
        Task RetainIngredients(string productId, IEnumerable<string> ingredientIds, CancellationToken token = default);
        
        /// <summary>
        /// Set available options to product, will replace existing ones
        /// </summary>
        /// <param name="productId">ID of product to set to</param>
        /// <param name="optionIds">Collection of option IDs to be set</param>
        /// <param name="token">Token for operation cancel</param>
        Task SetOptions(string productId, IEnumerable<string> optionIds, CancellationToken token = default);
        
        /// <summary>
        /// Assign available options to product
        /// </summary>
        /// <param name="productId">ID of product to assign to</param>
        /// <param name="optionIds">Collection of option IDs to be assigned</param>
        /// <param name="token">Token for operation cancel</param>
        Task AssignOptions(string productId, IEnumerable<string> optionIds, CancellationToken token = default);

        /// <summary>
        /// Retain available options from product
        /// </summary>
        /// <param name="productId">ID of option to retain from</param>
        /// <param name="optionIds">Collection of option IDs to be retained</param>
        /// <param name="token">Token for operation cancel</param>
        Task RetainOptions(string productId, IEnumerable<string> optionIds, CancellationToken token = default);
    }
}