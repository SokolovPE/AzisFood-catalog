using System.Collections;
using System.Collections.Generic;
using Catalog.DataAccess.Attributes;

namespace Catalog.DataAccess.Models
{
    /// <summary>
    /// Model of product presented in catalog.
    /// </summary>
    [BusCacheTopic(Name = "recache.product")]
    public class Product : MongoRepoEntity
    {
        /// <summary>
        /// Title of product.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Description of product.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Url of image cover.
        /// </summary>
        public string ImageUrl { get; set; }
        
        /// <summary>
        /// Price of product.
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// Categories where product is presented.
        /// </summary>
        public string[] CategoryId { get; set; }
        
        /// <summary>
        /// Nutrition facts for 100g of product.
        /// </summary>
        public NutritionFact NutritionFact { get; set; }
        
        /// <summary>
        /// Total weight of product.
        /// </summary>
        public double ServingSize { get; set; }
        
        /// <summary>
        /// Ingredients product made of.
        /// </summary>
        public IEnumerable<IngredientUsage> Ingredients { get; set; }
        
        /// <summary>
        /// Options available for product.
        /// </summary>
        public string[] OptionId { get; set; }
    }
}