using System.Globalization;

namespace Catalog.Sdk.Models
{
    /// <summary>
    /// Data transfer object for ingredient and it's usage
    /// </summary>
    public record IngredientUsageDto
    {
        /// <summary>
        /// Identifier of used ingredient
        /// </summary>
        public string IngredientId { get; set; }
        
        /// <summary>
        /// Amount of used ingredient
        /// </summary>
        public decimal Amount { get; set; }
        
        /// <summary>
        /// Marker can ingredient be removed or not
        /// </summary>
        public bool Toggleable { get; set; }
        
        public override string ToString()
        {
            return
                $"id: {IngredientId}, amount: {Amount.ToString(CultureInfo.InvariantCulture)} toggleable: {(Toggleable ? "yes" : "no")}";
        }
    }
}