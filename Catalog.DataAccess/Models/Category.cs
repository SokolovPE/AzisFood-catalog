﻿using AzisFood.CacheService.Abstractions.Models;
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
        /// Title of product
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Possible subcategories
        /// </summary>
        public string[] SubCategories { get; set; }
    }
}