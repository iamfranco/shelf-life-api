using Shelf.Life.Domain.Models;

namespace Shelf.Life.Domain.Services;
public interface IStorageItemService
{
    Task<IEnumerable<StorageItem>> Get();
    Task Insert(CreateStorageItemRequest request);
}
