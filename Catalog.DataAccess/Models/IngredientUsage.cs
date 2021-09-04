namespace Catalog.DataAccess.Models
{
    /// <summary>
    /// Usage of ingredient in product
    /// </summary>
    public class IngredientUsage
    {
        public string IngredientId { get; set; }
        public decimal Amount { get; set; }
    }
}