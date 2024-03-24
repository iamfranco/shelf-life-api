using Microsoft.Extensions.DependencyInjection;

namespace Shelf.Life.Domain.IoC;
public static class ServiceCollection
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        return services;
    }
}
