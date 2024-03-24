using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Models.Requests;

namespace Shelf.Life.Domain.Stores;
public interface IFoodStore
{
    Food? FindByName(string name);
    Food? FindById(int id);
    IEnumerable<Food> Get();
    IEnumerable<Food> QueryByPartialName(string partialName);
    Task Insert(CreateOrUpdateFoodRequest request);
    Task Update(int id, CreateOrUpdateFoodRequest request);
    Task Delete(int id);
}
