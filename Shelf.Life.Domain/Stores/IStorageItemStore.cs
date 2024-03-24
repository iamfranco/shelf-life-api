using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Models.Requests;

namespace Shelf.Life.Domain.Stores;
public interface IStorageItemStore
{
    Task<IEnumerable<StorageItem>> Get();
    Task Insert(CreateStorageItemRequest request);
}
