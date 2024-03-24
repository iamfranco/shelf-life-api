using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Models.Requests;

namespace Shelf.Life.Domain.Stores;
public interface IFoodStore
{
    Food FindById(int id);
    IEnumerable<Food> Get();
    IEnumerable<Food> QueryByPartialName(string partialName);
    Task<Food> Insert(CreateOrUpdateFoodRequest request);
    Task<Food> Update(int id, CreateOrUpdateFoodRequest request);
    Task Delete(int id);
}
