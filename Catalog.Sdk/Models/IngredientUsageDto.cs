namespace Catalog.Sdk.Models
{
    /// <summary>
    /// Data transfer object for ingredient and it's usage
    /// </summary>
    public record IngredientUsageDto
    {
        public string IngredientId { get; set; }
        public decimal Amount { get; set; }
    }
}