using Microsoft.EntityFrameworkCore;
using Shelf.Life.Database.Models;

namespace Shelf.Life.Database.Contexts;
public class DatabaseContext : DbContext
{
    public virtual DbSet<FoodDto> Foods { get; set; }
    public virtual DbSet<StorageItemDto> StorageItems { get; set; }
    public string DbPath { get; }
    public DatabaseContext()
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        path = Path.Combine(path, "Shelf.Life.Api");
        Directory.CreateDirectory(path);

        DbPath = Path.Join(path, "shelf_life.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }
}
