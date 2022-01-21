using System;

namespace Catalog.DataAccess.Models
{
    /// <summary>
    /// Usage of ingredient in product
    /// </summary>
    public class IngredientUsage
    {
        /// <summary>
        /// Identifier of used ingredient
        /// </summary>
        public Guid IngredientId { get; set; }
        
        /// <summary>
        /// Amount of used ingredient
        /// </summary>
        public decimal Amount { get; set; }
        
        /// <summary>
        /// Marker can ingredient be removed or not
        /// </summary>
        public bool Toggleable { get; set; }
    }
}