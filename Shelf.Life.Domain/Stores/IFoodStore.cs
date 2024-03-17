using Shelf.Life.Domain.Models;

namespace Shelf.Life.Domain.Stores;
public interface IFoodStore
{
    Task<Food?> FindByName(string name);
    Task<IEnumerable<Food>> Get();
    Task Insert(CreateFoodRequest request);
}
