using Catalog.DataAccess.Attributes;

namespace Catalog.DataAccess.Models
{
    /// <summary>
    /// Model of ingredient
    /// </summary>
    [BusTopic(Name = "ingredient", Events = new [] {EventType.Deleted, EventType.Recache})]
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