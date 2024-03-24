using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Models.Requests;

namespace Shelf.Life.Domain.Services;
public interface IStorageItemService
{
    Task<IEnumerable<StorageItem>> Get();
    Task Insert(CreateStorageItemRequest request);
}
