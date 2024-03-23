namespace Shelf.Life.Domain.Models;
public record Food(
    int Id,
    string Name,
    DateTime LastUsed
);
