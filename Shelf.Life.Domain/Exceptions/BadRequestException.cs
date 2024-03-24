namespace Shelf.Life.Domain.Exceptions;

public class BadRequestException(string message) : Exception(message)
{
}
