using MusicMan.Domain.Entities;

namespace MusicMan.Application.Abstractions.Persistence;

public interface IAlbumRepository
{
    Task<Album?> GetByDiscogsIdAsync(long discogsReleaseId, CancellationToken cancellationToken = default);
    Task<Album?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    Task AddAsync(Album album, CancellationToken cancellationToken = default);
}
