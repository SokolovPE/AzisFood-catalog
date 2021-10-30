using System.Globalization;

namespace Catalog.Sdk.Models
{
    /// <summary>
    /// Data transfer object for ingredient and it's usage
    /// </summary>
    public record IngredientUsageDto
    {
        public string IngredientId { get; set; }
        public decimal Amount { get; set; }
        public override string ToString()
        {
            return $"id: {IngredientId}, amount: {Amount.ToString(CultureInfo.InvariantCulture)}";
        }
    }
}