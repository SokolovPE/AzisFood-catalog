using AzisFood.DataEngine.Mongo.Models;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Catalog.DataAccess.Models
{
    /// <summary>
    /// Product nutrition facts per 100g
    /// </summary>
    public class NutritionFact : MongoRepoEntity
    {
        /// <summary>
        /// Hide original id from entity
        /// </summary>
        [BsonIgnore]
        [JsonIgnore]
        internal new string Id { get; set; }
        
        // Energy in kJoules
        public double Energy { get; set; }
        
        // Calories in kCal
        public double Calories { get; set; }
        
        // Fat amount
        public double TotalFat { get; set; }
        
        // Carbohydrates amount
        public double Carbohydrates { get; set; }
        
        // Protein amount
        public double Proteins { get; set; }
        
        // Sugar amount
        public double TotalSugar { get; set; }
    }
}