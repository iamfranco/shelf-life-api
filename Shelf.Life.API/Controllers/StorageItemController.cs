using Microsoft.AspNetCore.Mvc;
using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Services;
using System.Net;

namespace Shelf.Life.API.Controllers;

[Route("api/storageItems")]
public class StorageItemController : Controller
{
    private readonly IStorageItemService _storageItemService;

    public StorageItemController(IStorageItemService storageItemService)
    {
        _storageItemService = storageItemService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var storageItems = await _storageItemService.Get();
            return Ok(storageItems);
        }
        catch (Exception exception)
        {
            var value = $"{nameof(StorageItemService)}:GET throws unexpected exception: {exception.Message}";
            return StatusCode((int)HttpStatusCode.InternalServerError, value);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStorageItemRequest request)
    {
        await _storageItemService.Insert(request);
        return NoContent();
    }
}
