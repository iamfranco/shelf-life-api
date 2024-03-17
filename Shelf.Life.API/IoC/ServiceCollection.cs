using Shelf.Life.Domain.IoC;
using Shelf.Life.Database.IoC;

namespace Shelf.Life.API.IoC;

public static class ServiceCollection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddDomainServices();
        services.AddDatabaseServices();

        return services;
    }
}
