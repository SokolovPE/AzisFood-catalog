namespace Catalog.Sdk.Models;

public record NutritionFactDto
{
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