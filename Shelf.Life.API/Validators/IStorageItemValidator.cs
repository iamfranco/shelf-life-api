namespace Shelf.Life.API.Validators;

public interface IStorageItemValidator
{
    void ThrowIfStorageItemDoesNotExist(int id);
}
