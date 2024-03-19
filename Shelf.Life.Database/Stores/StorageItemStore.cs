using Shelf.Life.Database.Contexts;
using Shelf.Life.Database.Models;
using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Stores;

namespace Shelf.Life.Database.Stores;
public class StorageItemStore : IStorageItemStore
{
    private readonly DatabaseContext _context;

    public StorageItemStore(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StorageItem>> Get()
    {
        return _context.StorageItems.Select(storageItemDto => storageItemDto.ToStorageItem());
    }

    public async Task Insert(CreateStorageItemRequest request)
    {
        var storageItemDto = StorageItemDto.FromRequest(request);
        await _context.StorageItems.AddAsync(storageItemDto);
        await _context.SaveChangesAsync();
    }
}
