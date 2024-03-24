using Microsoft.AspNetCore.Mvc;
using Shelf.Life.Domain.Models.Requests;
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
            return StatusCode((int)HttpStatusCode.InternalServerError,
                $"{nameof(FoodController)}:{nameof(GetAll)} throws unexpected exception: {exception.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrUpdateFoodRequest request)
    {
        try
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
        catch (Exception exception)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                $"{nameof(FoodController)}:{nameof(Create)} throws unexpected exception: {exception.Message}");
        }
    }

    [HttpGet("partialName/{partialName}")]
    public IActionResult GetByPartialName(string partialName)
    {
        try
        {
            var matchingFoods = _foodStore.QueryByPartialName(partialName);
            return Ok(matchingFoods);
        }
        catch (Exception exception)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                $"{nameof(FoodController)}:{nameof(GetByPartialName)} throws unexpected exception: {exception.Message}");
        }
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        try
        {
            var matchingFood = _foodStore.FindById(id);
            if (matchingFood is null)
            {
                return NotFound();
            }
        
            return Ok(matchingFood);
        }
        catch (Exception exception)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                $"{nameof(FoodController)}:{nameof(Get)} throws unexpected exception: {exception.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateOrUpdateFoodRequest request)
    {
        try
        {
            var matchingFood = _foodStore.FindById(id);
            if (matchingFood is null)
            {
                return NotFound($"{nameof(FoodController)}:{nameof(Update)} failed. Food with id {id} not found.");
            }

            await _foodStore.Update(id, request);
            return NoContent();
        }
        catch (Exception exception)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                $"{nameof(FoodController)}:{nameof(Update)} throws unexpected exception: {exception.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var matchingFood = _foodStore.FindById(id);
            if (matchingFood is null)
            {
                return NotFound($"{nameof(FoodController)}:{nameof(Delete)} failed. Food with id {id} not found.");
            }

            await _foodStore.Delete(id);
            return NoContent();
        }
        catch (Exception exception)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                $"{nameof(FoodController)}:{nameof(Delete)} throws unexpected exception: {exception.Message}");
        }
    }
}
