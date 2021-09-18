using AzisFood.CacheService.Abstractions.Models;
using AzisFood.DataEngine.Mongo.Models;

namespace Catalog.DataAccess.Models
{
    /// <summary>
    /// Model of product category
    /// </summary>
    public class Category: MongoRepoEntity
    {
        [HashEntryKey]
        public string Code => Title.Replace(" ", "");
        
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