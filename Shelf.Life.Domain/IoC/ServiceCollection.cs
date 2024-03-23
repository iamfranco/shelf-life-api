using Microsoft.Extensions.DependencyInjection;
using Shelf.Life.Domain.Services;

namespace Shelf.Life.Domain.IoC;
public static class ServiceCollection
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddTransient<IStorageItemService, StorageItemService>();

        return services;
    }
}
