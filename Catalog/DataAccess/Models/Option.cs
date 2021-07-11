namespace Catalog.DataAccess.Models
{
    /// <summary>
    /// Model of product option.
    /// </summary>
    public class Option : MongoRepoEntity
    {
        /// <summary>
        /// Title of option.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Price of option.
        /// </summary>
        public decimal Price { get; set; }
    }
}