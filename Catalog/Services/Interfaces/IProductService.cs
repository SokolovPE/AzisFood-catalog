using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.DataAccess.Models;
using Catalog.Dto;

namespace Catalog.Services.Interfaces
{
    /// <summary>
    /// Service to operate products.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Get all products presented in catalog.
        /// </summary>
        /// <returns>List of products</returns>
        Task<List<ProductDto>> GetAllProductsAsync();

        /// <summary>
        /// Get product from catalog by id.
        /// </summary>
        /// <param name="id">Identifier of product.</param>
        /// <returns>Instance of <see cref="ProductDto"/>.</returns>
        Task<ProductDto> GetProductByIdAsync(string id);

        /// <summary>
        /// Add new product to catalog.
        /// </summary>
        /// <param name="product">Product to add.</param>
        /// <returns>Insertion result.</returns>
        Task<ProductDto> AddProductAsync(ProductRequestDto product);

        /// <summary>
        /// Update product in catalog.
        /// </summary>
        /// <param name="id">Identifier of product to be updated.</param>
        /// <param name="product">Product to be updated.</param>
        /// <returns>Insertion result.</returns>
        Task UpdateProductAsync(string id, ProductRequestDto product);

        /// <summary>
        /// Delete product from catalog.
        /// </summary>
        /// <param name="id">Identifier of product to be deleted.</param>
        /// <returns>Deletion result.</returns>
        Task<Product> DeleteProductAsync(string id);

        /// <summary>
        /// Delete products from catalog.
        /// </summary>
        /// <param name="ids">Array with identifiers of product to be deleted.</param>
        /// <returns>Deletion result.</returns>
        Task DeleteProductsAsync(string[] ids);
    }
}