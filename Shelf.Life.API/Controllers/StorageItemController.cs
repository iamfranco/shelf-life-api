using Microsoft.AspNetCore.Mvc;
using Shelf.Life.Domain.Models.Requests;
using Shelf.Life.Domain.Stores;

namespace Shelf.Life.API.Controllers;

[Route("api/storageItems")]
public class StorageItemController : Controller
{
    private readonly IStorageItemStore _storageItemStore;

    public StorageItemController(IStorageItemStore storageItemStore)
    {
        _storageItemStore = storageItemStore;
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
        var createdStorageItem = await _storageItemStore.Insert(request);
        return Ok(createdStorageItem);
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var matchingStorageItem = _storageItemStore.FindById(id);
        return Ok(matchingStorageItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateOrUpdateStorageItemRequest request)
    {
        var updatedStorageItem = await _storageItemStore.Update(id, request);
        return Ok(updatedStorageItem);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _storageItemStore.Delete(id);
        return NoContent();
    }
}
