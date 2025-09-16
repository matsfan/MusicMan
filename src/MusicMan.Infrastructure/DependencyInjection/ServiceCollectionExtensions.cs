using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicMan.Infrastructure.Persistence;
using MusicMan.Infrastructure.External.Discogs;
using MusicMan.Application.Abstractions.Services;
using MusicMan.Application.Abstractions.Persistence;

namespace MusicMan.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    private const string DefaultConnectionName = "DefaultConnection";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(DefaultConnectionName)
                               ?? "Data Source=musicman_dev.db"; // fallback for dev

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(connectionString, sqlite =>
            {
                // Keep migrations in Infrastructure by default
                sqlite.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
            });
        });

        // Add a DB health check
        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>(name: "db");

        // Repositories and unit of work
        services.AddScoped<IAlbumRepository, MusicMan.Infrastructure.Persistence.Repositories.AlbumRepository>();
        services.AddScoped<ICollectionRepository, MusicMan.Infrastructure.Persistence.Repositories.CollectionRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        // Discogs client
        services.Configure<DiscogsClientOptions>(configuration.GetSection("Discogs"));
        services.AddHttpClient<IDiscogsClient, DiscogsClient>();

        return services;
    }
}
