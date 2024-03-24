using Newtonsoft.Json;
using Shelf.Life.API.Validators.Models;
using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Stores;

namespace Shelf.Life.API.Validators;

public class FoodValidator : IFoodValidator
{
    private readonly IFoodStore _foodStore;

    public FoodValidator(IFoodStore foodStore)
    {
        _foodStore = foodStore;
    }

    public void ThrowIfFoodDoesNotExist(int id)
    {
        var matchingFood = _foodStore.FindById(id);
        if (matchingFood is null)
        {
            throw new NotFoundException($"{nameof(Food)} with id [{id}] does NOT exist.");
        }
    }

    public void ThrowIfFoodExists(string name)
    {
        var matchingFood = _foodStore.FindByName(name);
        if (matchingFood is not null)
        {
            throw new BadRequestException($"{nameof(Food)} with name [{name}] already exists. Food: {JsonConvert.SerializeObject(matchingFood)}");
        }
    }
}
