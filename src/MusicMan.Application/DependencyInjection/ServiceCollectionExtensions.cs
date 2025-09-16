using Microsoft.Extensions.DependencyInjection;
using MusicMan.Application.UseCases.Collection;

namespace MusicMan.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAddAlbumByBarcodeHandler, AddAlbumByBarcodeHandler>();
        return services;
    }
}
