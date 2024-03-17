using Shelf.Life.Domain.Models;

namespace Shelf.Life.Database.Models;
public class FoodDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime LastUsed { get; set; }
    public int TotalCalories { get; set; }
    public int TotalGrams { get; set; }
    public int CookingTimeMinutes { get; set; }

    public static FoodDto FromRequest(CreateFoodRequest request)
    {
        var foodDto = new FoodDto
        {
            Name = request.Name,
            LastUsed = DateTime.UtcNow,
            TotalCalories = request.TotalCalories,
            TotalGrams = request.TotalGrams,
            CookingTimeMinutes = request.CookingTimeMinutes
        };

        return foodDto;
    }

    public Food ToFood()
    {
        var food = new Food(
            Id,
            Name,
            LastUsed,
            TotalCalories,
            TotalGrams,
            CookingTimeMinutes
        );

        return food;
    }
}
