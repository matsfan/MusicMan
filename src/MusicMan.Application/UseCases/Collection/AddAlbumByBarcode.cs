using MusicMan.Application.Abstractions.Persistence;
using MusicMan.Application.Abstractions.Services;
using MusicMan.Domain.Entities;

namespace MusicMan.Application.UseCases.Collection;

public record AddAlbumByBarcodeCommand(string Barcode);

public record AddAlbumByBarcodeResult(Guid CollectionItemId, Guid AlbumId, long DiscogsReleaseId, string Title, string Artist);

public interface IAddAlbumByBarcodeHandler
{
    Task<AddAlbumByBarcodeResult?> HandleAsync(AddAlbumByBarcodeCommand command, CancellationToken cancellationToken = default);
}

public class AddAlbumByBarcodeHandler(
    IDiscogsClient discogs,
    IAlbumRepository albums,
    ICollectionRepository collection,
    IUnitOfWork uow
) : IAddAlbumByBarcodeHandler
{
    public async Task<AddAlbumByBarcodeResult?> HandleAsync(AddAlbumByBarcodeCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Barcode)) return null;

        // Check if album exists by barcode
        var existingByBarcode = await albums.GetByBarcodeAsync(command.Barcode, cancellationToken);
        Album album;

        if (existingByBarcode is not null)
        {
            album = existingByBarcode;
        }
        else
        {
            // Search Discogs by barcode
            var search = await discogs.SearchByBarcodeAsync(command.Barcode, cancellationToken);
            var best = search?.Results.FirstOrDefault();
            if (best is null) return null; // not found

            // Fetch release details for better metadata
            var release = await discogs.GetReleaseAsync(best.Id, cancellationToken);
            if (release is null) return null;

            // Check by Discogs id
            var existingByDiscogs = await albums.GetByDiscogsIdAsync(release.Id, cancellationToken);
            if (existingByDiscogs is not null)
            {
                album = existingByDiscogs;
            }
            else
            {
                album = new Album(
                    release.Id,
                    title: release.Title,
                    artist: release.Artists,
                    year: release.Year,
                    country: release.Country,
                    coverImageUrl: release.CoverImageUrl,
                    barcode: command.Barcode
                );
                await albums.AddAsync(album, cancellationToken);
            }
        }

        var item = new CollectionItem(album);
        await collection.AddAsync(item, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        return new AddAlbumByBarcodeResult(item.Id, album.Id, album.DiscogsReleaseId, album.Title, album.Artist);
    }
}
