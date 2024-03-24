using Shelf.Life.API.Validators.Models;
using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Stores;

namespace Shelf.Life.API.Validators;

public class StorageItemValidator : IStorageItemValidator
{
    private readonly IStorageItemStore _storageItemStore;

    public StorageItemValidator(IStorageItemStore storageItemStore)
    {
        _storageItemStore = storageItemStore;
    }

    public void ThrowIfStorageItemDoesNotExist(int id)
    {
        var matchingStorageItem = _storageItemStore.FindById(id);
        if (matchingStorageItem is null)
        {
            throw new NotFoundException($"{nameof(StorageItem)} with id [{id}] does NOT exist.");
        }
    }
}
