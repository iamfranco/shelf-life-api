using Shelf.Life.Domain.IoC;
using Shelf.Life.Database.IoC;
using Shelf.Life.API.Validators;

namespace Shelf.Life.API.IoC;

public static class ServiceCollection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddApiServices();
        services.AddDomainServices();
        services.AddDatabaseServices();

        return services;
    }

    private static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddTransient<IFoodValidator, FoodValidator>();

        return services;
    }
}
