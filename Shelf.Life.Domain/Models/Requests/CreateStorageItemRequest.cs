namespace Shelf.Life.Domain.Models.Requests;
public record CreateStorageItemRequest(
    int FoodId,
    DateTime ExpiryDate
);
