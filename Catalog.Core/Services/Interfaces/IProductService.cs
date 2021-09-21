using System.Collections.Generic;
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
        Task DeleteIngredients(IEnumerable<string> ingredientIds);

        /// <summary>
        /// Set categories to product, will replace existing ones
        /// </summary>
        /// <param name="productId">ID of product to set to</param>
        /// <param name="categoryIds">Array of category IDs to be set</param>
        Task SetCategories(string productId, IEnumerable<string> categoryIds);
        
        /// <summary>
        /// Assign additional categories to product
        /// </summary>
        /// <param name="productId">ID of product to assign to</param>
        /// <param name="categoryIds">Array of category IDs to be assigned</param>
        Task AssignCategories(string productId, IEnumerable<string> categoryIds);

        /// <summary>
        /// Retain categories from product
        /// </summary>
        /// <param name="productId">ID of product to retain from</param>
        /// <param name="categoryIds">Array of category IDs to be retained</param>
        Task RetainCategories(string productId, IEnumerable<string> categoryIds);
    }
}