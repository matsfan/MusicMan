using Microsoft.EntityFrameworkCore;
using MusicMan.Application.Abstractions.Persistence;
using MusicMan.Domain.Entities;

namespace MusicMan.Infrastructure.Persistence.Repositories;

public class AlbumRepository(AppDbContext db) : IAlbumRepository
{
    private readonly AppDbContext _db = db;

    public async Task AddAsync(Album album, CancellationToken cancellationToken = default)
    {
        await _db.Albums.AddAsync(album, cancellationToken);
    }

    public Task<Album?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
        => _db.Albums.FirstOrDefaultAsync(a => a.Barcode == barcode, cancellationToken);

    public Task<Album?> GetByDiscogsIdAsync(long discogsReleaseId, CancellationToken cancellationToken = default)
        => _db.Albums.FirstOrDefaultAsync(a => a.DiscogsReleaseId == discogsReleaseId, cancellationToken);
}
