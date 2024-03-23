using Microsoft.AspNetCore.Mvc;
using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Stores;
using System.Net;
using System.Text.Json;

namespace Shelf.Life.API.Controllers;

[Route("api/foods")]
public class FoodController : Controller
{
    private readonly IFoodStore _foodStore;

    public FoodController(IFoodStore foodStore)
    {
        _foodStore = foodStore;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        try
        {
            var foods = _foodStore.Get();
            return Ok(foods);
        }
        catch (Exception exception)
        {
            var value = $"{nameof(Food)}:GET throws unexpected exception: {exception.Message}";
            return StatusCode((int)HttpStatusCode.InternalServerError, value);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFoodRequest request)
    {
        var matchingFood = _foodStore.FindByName(request.Name);
        if (matchingFood is not null)
        {
            return BadRequest($"{nameof(Food)}:POST failed. Food with same name already exist, " +
                $"duplicate food: {JsonSerializer.Serialize(matchingFood)}");
        }

        await _foodStore.Insert(request);
        return NoContent();
    }

    [HttpGet("partialName/{partialName}")]
    public IActionResult GetByPartialName(string partialName)
    {
        var matchingFoods = _foodStore.QueryByPartialName(partialName);
        return Ok(matchingFoods);
    }
}
