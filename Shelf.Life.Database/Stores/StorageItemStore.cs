using Shelf.Life.Database.Contexts;
using Shelf.Life.Database.Models;
using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Models.Requests;
using Shelf.Life.Domain.Stores;

namespace Shelf.Life.Database.Stores;
public class StorageItemStore : IStorageItemStore
{
    private readonly DatabaseContext _context;

    public StorageItemStore(DatabaseContext context)
    {
        _context = context;
    }

    public async Task Delete(int id)
    {
        var storageItemDto = FindStorageItemDtoById(id);
        if (storageItemDto is null)
        {
            return;
        }

        _context.StorageItems.Remove(storageItemDto);
        await _context.SaveChangesAsync();
    }

    public StorageItem? FindById(int id)
    {
        return FindStorageItemDtoById(id)?.ToStorageItem();
    }

    public IEnumerable<StorageItem> Get()
    {
        return _context.StorageItems.Select(storageItemDto => storageItemDto.ToStorageItem());
    }

    public async Task Insert(CreateOrUpdateStorageItemRequest request)
    {
        var storageItemDto = StorageItemDto.FromRequest(request);
        await _context.StorageItems.AddAsync(storageItemDto);
        await _context.SaveChangesAsync();
    }

    public async Task Update(int id, CreateOrUpdateStorageItemRequest request)
    {
        var matchingStorageItem = FindStorageItemDtoById(id);
        if (matchingStorageItem is null)
        {
            return;
        }

        matchingStorageItem.Update(request);

        _context.StorageItems.Update(matchingStorageItem);
        await _context.SaveChangesAsync();
    }

    private StorageItemDto? FindStorageItemDtoById(int id)
    {
        var matchingStorageItem = _context.StorageItems.FirstOrDefault(storageItemDto => storageItemDto.Id == id);
        return matchingStorageItem;
    }
}
