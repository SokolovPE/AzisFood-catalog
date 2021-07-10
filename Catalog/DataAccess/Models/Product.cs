namespace Catalog.DataAccess.Models
{
    /// <summary>
    /// Model of product presented in catalog.
    /// </summary>
    public class Product : MongoRepoEntity
    {
        /// <summary>
        /// Title of product.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Description of product.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Url of image cover.
        /// </summary>
        public string ImgUrl { get; set; }
        
        /// <summary>
        /// Price of product.
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// Categories where product is presented.
        /// </summary>
        public string[] CategoryId { get; set; }
    }
}