namespace Shelf.Life.Domain.Exceptions;

public class NotFoundException(string message) : Exception(message)
{
}
