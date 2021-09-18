using AzisFood.DataEngine.Mongo.Models;

namespace Catalog.DataAccess.Models
{
    /// <summary>
    /// Model of product option
    /// </summary>
    public class Option : MongoRepoEntity
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