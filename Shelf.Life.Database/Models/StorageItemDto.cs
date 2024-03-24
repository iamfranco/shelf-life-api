using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Models.Requests;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shelf.Life.Database.Models;
public class StorageItemDto
{
    public int Id { get; set; }

    [ForeignKey(nameof(Food))]
    public int FoodId { get; set; }
    public DateTime ExpiryDate { get; set; }

    public static StorageItemDto FromRequest(CreateOrUpdateStorageItemRequest request)
    {
        var storageItemDto = new StorageItemDto
        {
            FoodId = request.FoodId,
            ExpiryDate = request.ExpiryDate,
        };

        return storageItemDto;
    }

    public StorageItem ToStorageItem()
    {
        var storageItem = new StorageItem(
            Id,
            FoodId,
            ExpiryDate
        );

        return storageItem;
    }

    public void Update(CreateOrUpdateStorageItemRequest request)
    {
        FoodId = request.FoodId;
        ExpiryDate = request.ExpiryDate;
    }
}
