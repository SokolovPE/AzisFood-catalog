namespace Catalog.Dto
{
    /// <summary>
    /// Request data transfer object for ingredient.
    /// </summary>
    public class IngredientRequestDto : IRequestDto
    {
        /// <summary>
        /// Title of ingredient.
        /// </summary>
        public string Title { get; set; }
    }
}