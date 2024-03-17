namespace Shelf.Life.Domain.Models;
public record CreateFoodRequest(
    string Name,
    int TotalCalories,
    int TotalGrams,
    int CookingTimeMinutes
);
