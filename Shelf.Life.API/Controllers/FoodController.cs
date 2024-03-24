using Microsoft.AspNetCore.Mvc;
using Shelf.Life.API.Validators;
using Shelf.Life.API.Validators.Models;
using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Models.Requests;
using Shelf.Life.Domain.Stores;

namespace Shelf.Life.API.Controllers;

[Route("api/foods")]
public class FoodController : Controller
{
    private readonly IFoodStore _foodStore;
    private readonly IFoodValidator _foodValidator;

    public FoodController(IFoodStore foodStore, IFoodValidator foodValidator)
    {
        _foodStore = foodStore;
        _foodValidator = foodValidator;
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
        _foodValidator.ThrowIfFoodExists(request.Name);

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
            throw new NotFoundException($"{nameof(Food)} with id [{id}] does NOT exist.");
        }

        return Ok(matchingFood);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateOrUpdateFoodRequest request)
    {
        _foodValidator.ThrowIfFoodDoesNotExist(id);

        await _foodStore.Update(id, request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _foodValidator.ThrowIfFoodDoesNotExist(id);

        await _foodStore.Delete(id);
        return NoContent();
    }
}
