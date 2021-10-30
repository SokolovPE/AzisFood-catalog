namespace Catalog.Sdk.Models
{
    /// <summary>
    /// Data transfer object for category
    /// </summary>
    public record CategoryDto : CategoryRequestDto, IDto
    {
        /// <summary>
        /// Identifier of category
        /// </summary>
        public string Id { get; set; }
        
    }
}