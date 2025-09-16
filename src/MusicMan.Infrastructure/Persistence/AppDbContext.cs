using Microsoft.EntityFrameworkCore;

namespace MusicMan.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // TODO: Add DbSet<TEntity> properties as aggregates/entities are introduced
}
