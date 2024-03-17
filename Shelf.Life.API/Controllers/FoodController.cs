using Microsoft.AspNetCore.Mvc;

namespace Shelf.Life.API.Controllers;

[Route("api/foods")]
public class FoodController : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok();
    }
}
