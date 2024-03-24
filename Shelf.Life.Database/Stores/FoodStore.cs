using Newtonsoft.Json;
using Shelf.Life.Database.Contexts;
using Shelf.Life.Database.Models;
using Shelf.Life.Domain.Exceptions;
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

    public Food FindById(int id)
    {
        return TryFindDtoById(id).ToFood();
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

    public async Task<Food> Insert(CreateOrUpdateFoodRequest request)
    {
        ThrowIfHasMatchingFood(request.Name);

        var foodDto = FoodDto.FromRequest(request);
        _context.Foods.Add(foodDto);
        await _context.SaveChangesAsync();

        return foodDto.ToFood();
    }

    public async Task<Food> Update(int id, CreateOrUpdateFoodRequest request)
    {
        var matchingFoodDto = TryFindDtoById(id);

        matchingFoodDto.Update(request);

        _context.Foods.Update(matchingFoodDto);
        await _context.SaveChangesAsync();

        return matchingFoodDto.ToFood();
    }

    public async Task Delete(int id)
    {
        var matchingFoodDto = TryFindDtoById(id);

        _context.Foods.Remove(matchingFoodDto);
        await _context.SaveChangesAsync();
    }

    private FoodDto TryFindDtoById(int id)
    {
        var matchingFoodDto = _context.Foods.FirstOrDefault(foodDto => foodDto.Id == id);
        if (matchingFoodDto is null)
        {
            throw new NotFoundException($"{nameof(Food)} with id [{id}] does NOT exist.");
        }
        
        return matchingFoodDto;
    }

    private void ThrowIfHasMatchingFood(string name)
    {
        var matchingFoodDto = _context.Foods.FirstOrDefault(foodDto => foodDto.Name == name);
        if ( matchingFoodDto is not null)
        {
            throw new BadRequestException($"{nameof(Food)} with name [{name}] already exist. Food: {JsonConvert.SerializeObject(matchingFoodDto)}");
        }
    }
}
