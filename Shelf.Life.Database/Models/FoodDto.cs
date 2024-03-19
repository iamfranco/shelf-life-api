using Shelf.Life.Domain.Models;

namespace Shelf.Life.Database.Models;
public class FoodDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime LastUsed { get; set; }

    public static FoodDto FromRequest(CreateFoodRequest request)
    {
        var foodDto = new FoodDto
        {
            Name = request.Name,
            LastUsed = DateTime.UtcNow
        };

        return foodDto;
    }

    public Food ToFood()
    {
        var food = new Food(
            Id,
            Name,
            LastUsed
        );

        return food;
    }
}
