using System;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Core.Services.Interfaces;
using Catalog.DataAccess.Models;
using Catalog.Sdk.Models;

namespace Catalog.Core.Services.Implementations
{
    /// <summary>
    /// Service to validate product.
    /// </summary>
    public class ProductValidatorService : IValidatorService<Product>
    {
        private readonly IService<Ingredient, IngredientDto, IngredientRequestDto> _ingredientService;

        /// <summary>
        /// Validator of products.
        /// </summary>
        /// <param name="ingredientService">Service to operate ingredients.</param>
        public ProductValidatorService(IService<Ingredient, IngredientDto, IngredientRequestDto> ingredientService)
        {
            _ingredientService = ingredientService;
        }

        /// <summary>
        /// Validate object for errors.
        /// </summary>
        /// <param name="item">Object to validate.</param>
        /// <returns>Validation result.</returns>
        public async Task<Tuple<bool, string>> Validate(Product item)
        {
            if (item.ServingSize <= 0)
            {
                return new Tuple<bool, string>(false, "Serving size must be greater than zero");
            }
            
            var items = await _ingredientService.GetAllAsync();
            if (items.Count == 0)
            {
                return new Tuple<bool, string>(false, "Cannot fetch ingredients");
            }
            
            var ingredientIds = item.Ingredients.Select(x => x.IngredientId);
            var result = !ingredientIds.Except(items.Select(x=>x.Id)).Any();
            return new Tuple<bool, string>(result,
                result ? string.Empty : "Some of listed ingredients are not presented at server");
        }
    }
}