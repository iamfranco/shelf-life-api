namespace Shelf.Life.API.Validators;

public interface IFoodValidator
{
    void ThrowIfFoodDoesNotExist(int id);
    void ThrowIfFoodExists(string name);
}
