using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Stores;

namespace Shelf.Life.Domain.Services;
public class FoodService : IFoodService
{
    private readonly IFoodStore _foodStore;

    public FoodService(IFoodStore foodStore)
    {
        _foodStore = foodStore;
    }

    public async Task<Food?> FindMatchingFood(CreateFoodRequest request)
    {
        return await _foodStore.FindByName(request.Name);
    }

    public async Task<IEnumerable<Food>> Get()
    {
        return await _foodStore.Get();
    }

    public async Task Insert(CreateFoodRequest request)
    {
        var matchingFood = await _foodStore.FindByName(request.Name);
        if (matchingFood is not null)
        {
            return;
        }

        await _foodStore.Insert(request);
    }
}
