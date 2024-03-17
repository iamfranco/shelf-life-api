using Microsoft.Extensions.DependencyInjection;
using Shelf.Life.Database.Contexts;
using Shelf.Life.Database.Stores;
using Shelf.Life.Domain.Stores;

namespace Shelf.Life.Database.IoC;
public static class ServiceCollection
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
    {
        services.ConfigureDatabases();

        services.AddTransient<IFoodStore, FoodStore>();

        return services;
    }

    private static void ConfigureDatabases(this IServiceCollection services)
    {
        services.AddDbContext<DatabaseContext>();
    }
}
