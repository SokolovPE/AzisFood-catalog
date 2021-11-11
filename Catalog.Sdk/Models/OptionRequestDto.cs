namespace Catalog.Sdk.Models
{
    /// <summary>
    /// Request data transfer object for option
    /// </summary>
    public record OptionRequestDto : IRequestDto
    {
        /// <summary>
        /// Title of option
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Url of image cover
        /// </summary>
        public string ImageUrl { get; set; }
        
        /// <summary>
        /// Price of option
        /// </summary>
        public decimal Price { get; set; }
    }
}