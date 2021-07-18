﻿using Catalog.DataAccess.Attributes;

namespace Catalog.DataAccess.Models
{
    /// <summary>
    /// Model of ingredient.
    /// </summary>
    [BusCacheTopic(Name = "recache.ingredient")]
    public class Ingredient : MongoRepoEntity
    {
        /// <summary>
        /// Title of ingredient.
        /// </summary>
        public string Title { get; set; }
    }
}