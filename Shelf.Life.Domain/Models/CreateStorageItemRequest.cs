namespace Shelf.Life.Domain.Models;
public record CreateStorageItemRequest(
    int FoodId,
    DateTime ExpiryDate
);
