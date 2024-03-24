using Microsoft.AspNetCore.Mvc;
using Shelf.Life.Domain.Models.Requests;
using Shelf.Life.Domain.Stores;
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
        var foods = _foodStore.Get();
        return Ok(foods);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrUpdateFoodRequest request)
    {
        var matchingFood = _foodStore.FindByName(request.Name);
        if (matchingFood is not null)
        {
            return BadRequest($"{nameof(FoodController)}:{nameof(Create)} failed. Food with same name already exist, " +
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

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var matchingFood = _foodStore.FindById(id);
        if (matchingFood is null)
        {
            return NotFound();
        }

        return Ok(matchingFood);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateOrUpdateFoodRequest request)
    {
        var matchingFood = _foodStore.FindById(id);
        if (matchingFood is null)
        {
            return NotFound($"{nameof(FoodController)}:{nameof(Update)} failed. Food with id {id} not found.");
        }

        await _foodStore.Update(id, request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var matchingFood = _foodStore.FindById(id);
        if (matchingFood is null)
        {
            return NotFound($"{nameof(FoodController)}:{nameof(Delete)} failed. Food with id {id} not found.");
        }

        await _foodStore.Delete(id);
        return NoContent();
    }
}
