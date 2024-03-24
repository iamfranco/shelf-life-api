using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Models.Requests;

namespace Shelf.Life.Domain.Stores;
public interface IStorageItemStore
{
    Task Delete(int id);
    StorageItem? FindById(int id);
    IEnumerable<StorageItem> Get();
    Task Insert(CreateOrUpdateStorageItemRequest request);
    Task Update(int id, CreateOrUpdateStorageItemRequest request);
}
