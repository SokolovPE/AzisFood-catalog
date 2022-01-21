using System;

namespace Catalog.Sdk.Models
{
    /// <summary>
    /// Data transfer object for product
    /// </summary>
    public record ProductDto : ProductRequestDto, IDto
    {
        /// <summary>
        /// Identifier of product
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Nutrition facts for serving size of product
        /// </summary>
        public NutritionFactDto TotalNutritionFact
        {
            get
            {
                // TotalWeight(g) / 100(g) and then multiply nutrition fact values
                var multiplier = ServingSize / 100;
                return new NutritionFactDto
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