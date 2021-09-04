namespace Catalog.Sdk.Models
{
    /// <summary>
    /// Data transfer object for ingredient
    /// </summary>
    public class IngredientDto : IngredientRequestDto, IDto
    {
        /// <summary>
        /// Identifier of ingredient
        /// </summary>
        public string Id { get; set; }
    }
}