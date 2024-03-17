using Microsoft.AspNetCore.Mvc;
using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Services;
using System.Net;
using System.Text.Json;

namespace Shelf.Life.API.Controllers;

[Route("api/foods")]
public class FoodController : Controller
{
    private readonly IFoodService _foodService;

    public FoodController(IFoodService foodService)
    {
        _foodService = foodService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var foods = await _foodService.Get();
            return Ok(foods);
        }
        catch (Exception exception)
        {
            var value = $"{nameof(FoodService)}:GET throws unexpected exception: {exception.Message}";
            return StatusCode((int)HttpStatusCode.InternalServerError, value);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFoodRequest request)
    {
        var matchingFood = await _foodService.FindMatchingFood(request);
        if (matchingFood is not null)
        {
            return BadRequest($"{nameof(FoodService)}:POST failed. Food with same name already exist, " +
                $"duplicate food: {JsonSerializer.Serialize(matchingFood)}");
        }

        await _foodService.Insert(request);
        return NoContent();
    }
}
