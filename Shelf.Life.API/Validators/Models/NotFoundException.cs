namespace Shelf.Life.API.Validators.Models;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
