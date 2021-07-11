﻿using Catalog.DataAccess.Models;

namespace Catalog.Dto
{
    /// <summary>
    /// Data transfer object for product.
    /// </summary>
    public class ProductDto : ProductRequestDto
    {
        /// <summary>
        /// Identifier of product.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Nutrition facts for serving size of product.
        /// </summary>
        public NutritionFact TotalNutritionFact
        {
            get
            {
                // TotalWeight(g) / 100(g) and then multiply nutrition fact values.
                var multiplier = ServingSize / 100;
                return new NutritionFact
                {
                    Calories = NutritionFact.Calories * multiplier,
                    Carbohydrates = NutritionFact.Carbohydrates * multiplier,
                    Energy = NutritionFact.Energy * multiplier,
                    Proteins = NutritionFact.Proteins * multiplier,
                    TotalFat = NutritionFact.TotalFat * multiplier,
                    TotalSugar = NutritionFact.TotalSugar * multiplier
                };
            }
        }
    }
}