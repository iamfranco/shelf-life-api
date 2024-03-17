namespace Shelf.Life.Domain.Models;
public record Food(
    int Id,
    string Name,
    DateTime LastUsed,
    int TotalCalories,
    int TotalGrams,
    int CookingTimeMinutes
);
