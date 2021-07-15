using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.DataAccess.Models
{
    public abstract class MongoRepoEntity
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        protected MongoRepoEntity()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }
    }
}