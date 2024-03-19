using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Stores;

namespace Shelf.Life.Domain.Services;
public class StorageItemService : IStorageItemService
{
    private readonly IStorageItemStore _storageItemStore;

    public StorageItemService(IStorageItemStore storageItemStore)
    {
        _storageItemStore = storageItemStore;
    }

    public async Task<IEnumerable<StorageItem>> Get()
    {
        return await _storageItemStore.Get();
    }

    public async Task Insert(CreateStorageItemRequest request)
    {
        await _storageItemStore.Insert(request);
    }
}
