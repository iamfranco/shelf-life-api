namespace Shelf.Life.Domain.Models.Requests;
public record CreateOrUpdateStorageItemRequest(
    int FoodId,
    DateTime ExpiryDate
);
