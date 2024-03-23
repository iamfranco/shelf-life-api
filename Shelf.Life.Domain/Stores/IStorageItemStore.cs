using Shelf.Life.Domain.Models;

namespace Shelf.Life.Domain.Stores;
public interface IStorageItemStore
{
    Task<IEnumerable<StorageItem>> Get();
    Task Insert(CreateStorageItemRequest request);
}
