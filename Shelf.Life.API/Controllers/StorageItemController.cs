using Microsoft.AspNetCore.Mvc;
using Shelf.Life.API.Validators;
using Shelf.Life.API.Validators.Models;
using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Models.Requests;
using Shelf.Life.Domain.Stores;

namespace Shelf.Life.API.Controllers;

[Route("api/storageItems")]
public class StorageItemController : Controller
{
    private readonly IStorageItemStore _storageItemStore;
    private readonly IStorageItemValidator _storageItemValidator;

    public StorageItemController(IStorageItemStore storageItemStore, IStorageItemValidator storageItemValidator)
    {
        _storageItemStore = storageItemStore;
        _storageItemValidator = storageItemValidator;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var storageItems = _storageItemStore.Get();
        return Ok(storageItems);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrUpdateStorageItemRequest request)
    {
        await _storageItemStore.Insert(request);
        return NoContent();
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var matchingStorageItem = _storageItemStore.FindById(id);
        if (matchingStorageItem is null)
        {
            throw new NotFoundException($"{nameof(StorageItem)} with id [{id}] does NOT exist.");
        }

        return Ok(matchingStorageItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateOrUpdateStorageItemRequest request)
    {
        _storageItemValidator.ThrowIfStorageItemDoesNotExist(id);

        await _storageItemStore.Update(id, request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _storageItemValidator.ThrowIfStorageItemDoesNotExist(id);

        await _storageItemStore.Delete(id);
        return NoContent();
    }
}
