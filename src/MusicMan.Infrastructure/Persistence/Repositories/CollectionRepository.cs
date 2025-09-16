using MusicMan.Application.Abstractions.Persistence;
using MusicMan.Domain.Entities;

namespace MusicMan.Infrastructure.Persistence.Repositories;

public class CollectionRepository(AppDbContext db) : ICollectionRepository
{
    private readonly AppDbContext _db = db;

    public async Task AddAsync(CollectionItem item, CancellationToken cancellationToken = default)
    {
        await _db.CollectionItems.AddAsync(item, cancellationToken);
    }
}
