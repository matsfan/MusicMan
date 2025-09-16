using Microsoft.EntityFrameworkCore;
using MusicMan.Domain.Entities;
using MusicMan.Application.Abstractions.Persistence;

namespace MusicMan.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<CollectionItem> CollectionItems => Set<CollectionItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
