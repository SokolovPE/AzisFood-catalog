using System;
using AzisFood.CacheService.Abstractions.Models;
using AzisFood.DataEngine.Mongo.Models;
using AzisFood.MQ.Abstractions.Attributes;

namespace Catalog.DataAccess.Models
{
    /// <summary>
    /// Model of product category
    /// </summary>
    [BusTopic(Name = "category")]
    public class Category: MongoRepoEntity
    {
        /// <summary>
        /// Title of category
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Possible subcategories
        /// </summary>
        public Guid[] SubCategories { get; set; }
        
        /// <summary>
        /// Order of category
        /// </summary>
        public int Order { get; set; }
    }
}