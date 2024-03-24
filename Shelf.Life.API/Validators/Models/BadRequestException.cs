namespace Shelf.Life.API.Validators.Models;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }
}
