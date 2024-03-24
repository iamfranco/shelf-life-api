namespace Shelf.Life.API.Models;

public record ErrorResponse(
    string Error,
    string Method,
    string? Endpoint,
    string? Body
);
