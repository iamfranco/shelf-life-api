namespace Shelf.Life.Domain.Models;
public record StorageItem(
    int Id,
    int FoodId,
    DateTime ExpiryDate
);
