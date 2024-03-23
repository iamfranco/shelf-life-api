using Shelf.Life.Domain.Models;

namespace Shelf.Life.Domain.Stores;
public interface IFoodStore
{
    Food? FindByName(string name);
    IEnumerable<Food> Get();
    IEnumerable<Food> QueryByPartialName(string partialName);
    Task Insert(CreateFoodRequest request);
}
