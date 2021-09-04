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
    }
}