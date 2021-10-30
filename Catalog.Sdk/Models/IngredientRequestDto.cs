namespace Catalog.Sdk.Models
{
    /// <summary>
    /// Request data transfer object for ingredient
    /// </summary>
    public record IngredientRequestDto : IRequestDto
    {
        /// <summary>
        /// Title of ingredient
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Identifier of measure unit
        /// </summary>
        public string MeasureUnitId { get; set; }
    }
}