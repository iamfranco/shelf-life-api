using Microsoft.AspNetCore.Mvc;
using Shelf.Life.Domain.Models.Requests;
using Shelf.Life.Domain.Stores;

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
        var foodCreated = await _foodStore.Insert(request);
        return Ok(foodCreated);
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
        return Ok(matchingFood);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateOrUpdateFoodRequest request)
    {
        var foodUpdated = await _foodStore.Update(id, request);
        return Ok(foodUpdated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _foodStore.Delete(id);
        return NoContent();
    }
}
