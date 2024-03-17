using Shelf.Life.Domain.Models;

namespace Shelf.Life.Domain.Services;
public interface IFoodService
{
    Task<Food?> FindMatchingFood(CreateFoodRequest food);
    Task<IEnumerable<Food>> Get();
    Task Insert(CreateFoodRequest food);
}
