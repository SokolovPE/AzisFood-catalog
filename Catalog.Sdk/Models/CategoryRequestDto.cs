namespace Catalog.Sdk.Models
{
    /// <summary>
    /// Request data transfer object for category
    /// </summary>
    public record CategoryRequestDto : IRequestDto
    {
        /// <summary>
        /// Title of product
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Possible subcategories
        /// </summary>
        public string[] SubCategories { get; set; }
    }
}