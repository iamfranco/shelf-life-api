using Shelf.Life.Database.Contexts;
using Shelf.Life.Database.Models;
using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Models.Requests;
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

    public Food? FindById(int id)
    {
        return FindDtoById(id)?.ToFood();
    }

    public IEnumerable<Food> Get()
    {
        return _context.Foods.Select(foodDto => foodDto.ToFood());
    }

    public IEnumerable<Food> QueryByPartialName(string partialName)
    {
        return _context.Foods
            .Where(foodDto => foodDto.Name.ToLower().Contains(partialName.ToLower()))
            .Select(foodDto => foodDto.ToFood());
    }

    public async Task Insert(CreateOrUpdateFoodRequest request)
    {
        var foodDto = FoodDto.FromRequest(request);
        await _context.Foods.AddAsync(foodDto);
        await _context.SaveChangesAsync();
    }

    public async Task Update(int id, CreateOrUpdateFoodRequest request)
    {
        var matchingFoodDto = FindDtoById(id);
        if (matchingFoodDto is null)
        {
            return;
        }

        matchingFoodDto.Update(request);

        _context.Foods.Update(matchingFoodDto);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var matchingFoodDto = FindDtoById(id);
        if (matchingFoodDto is null)
        {
            return;
        }

        _context.Foods.Remove(matchingFoodDto);
        await _context.SaveChangesAsync();
    }

    private FoodDto? FindDtoById(int id)
    {
        var matchingFoodDto = _context.Foods.FirstOrDefault(foodDto => foodDto.Id == id);
        return matchingFoodDto;
    }
}
