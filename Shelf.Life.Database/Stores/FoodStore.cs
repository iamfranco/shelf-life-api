using Shelf.Life.Database.Contexts;
using Shelf.Life.Database.Models;
using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Stores;

namespace Shelf.Life.Database.Stores;
public class FoodStore : IFoodStore
{
    private readonly DatabaseContext _context;

    public FoodStore(DatabaseContext context)
    {
        _context = context;
    }

    public Food? FindByName(string name)
    {
        var matchingFoodDto = _context.Foods.FirstOrDefault(foodDto => foodDto.Name == name);
        return matchingFoodDto?.ToFood();
    }

    public IEnumerable<Food> Get()
    {
        return _context.Foods.Select(foodDto => foodDto.ToFood());
    }

    public IEnumerable<Food> QueryByPartialName(string partialName)
    {
        return _context.Foods
            .Where(foodDto => foodDto.Name.Contains(partialName))
            .Select(foodDto => foodDto.ToFood());
    }

    public async Task Insert(CreateFoodRequest request)
    {
        var foodDto = FoodDto.FromRequest(request);
        await _context.Foods.AddAsync(foodDto);
        await _context.SaveChangesAsync();
    }
}
