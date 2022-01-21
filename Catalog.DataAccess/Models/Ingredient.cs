﻿using AzisFood.CacheService.Abstractions.Models;
using AzisFood.DataEngine.Core;
using AzisFood.DataEngine.Core.Attributes;
using AzisFood.DataEngine.Mongo.Models;
using AzisFood.MQ.Abstractions.Attributes;
using AzisFood.MQ.Abstractions.Models;

namespace Catalog.DataAccess.Models
{
    /// <summary>
    /// Model of ingredient
    /// </summary>
    [BusTopic(Name = "ingredient", Events = new [] {EventType.Deleted, EventType.Recache})]
    // Key for demonstration how to define own name for HashSet, by default h_TypeName
    [HashKey(Key = "h_Ingredient")]
    [ConnectionAlias("catalog")]
    public class Ingredient : MongoRepoEntity
    {
        /// <summary>
        /// Title of ingredient
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Identifier of measure unit
        /// </summary>
        public string MeasureUnitId { get; set; }
    }
}