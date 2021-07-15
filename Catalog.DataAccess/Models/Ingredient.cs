namespace Catalog.DataAccess.Models
{
    /// <summary>
    /// Model of ingredient.
    /// </summary>
    public class Ingredient : MongoRepoEntity
    {
        /// <summary>
        /// Title of ingredient.
        /// </summary>
        public string Title { get; set; }
    }
}