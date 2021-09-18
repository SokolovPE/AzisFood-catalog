namespace Catalog.Sdk.Models
{
    /// <summary>
    /// Data transfer object for category
    /// </summary>
    public class CategoryDto : CategoryRequestDto, IDto
    {
        /// <summary>
        /// Identifier of category
        /// </summary>
        public string Id { get; set; }
        
    }
}