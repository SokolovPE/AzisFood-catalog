﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Catalog.DataAccess.Models;
using Catalog.Dto;
using Catalog.Services.Interfaces;

namespace Catalog.Services.Implementations
{
    /// <summary>
    /// Service to validate product.
    /// </summary>
    public class ProductValidatorService : IValidatorService<Product>
    {
        private readonly IService<Ingredient, IngredientDto, IngredientRequestDto> _ingredientService;

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
            var result = !items.Select(x => x.Id).Except(ingredientIds).Any();
            return new Tuple<bool, string>(result, result?string.Empty:"Some of listed ingredients are not presented at server");
        }
    }
}