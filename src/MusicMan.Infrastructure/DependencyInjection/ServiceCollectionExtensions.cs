using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicMan.Infrastructure.Persistence;

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

        return services;
    }
}
