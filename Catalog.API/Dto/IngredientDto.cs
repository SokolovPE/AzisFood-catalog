namespace Catalog.Dto
{
    public class IngredientDto : IngredientRequestDto, IDto
    {
        /// <summary>
        /// Identifier of ingredient.
        /// </summary>
        public string Id { get; set; }
    }
}