using MusicMan.Application.Abstractions.Persistence;
using MusicMan.Application.Abstractions.Services;
using MusicMan.Application.Abstractions.Services.Models;
using MusicMan.Application.UseCases.Collection;
using MusicMan.Domain.Entities;

namespace MusicMan.Application.Tests.Features.Collection;

public class AddAlbumByBarcodeTests
{
    [Fact]
    public async Task Adds_album_and_collection_item()
    {
        // Arrange
        var barcode = "1234567890123";
        var discogsId = 42L;

        var discogs = new FakeDiscogsClient(discogsId);
        var albumRepo = new InMemoryAlbumRepository();
        var collectionRepo = new InMemoryCollectionRepository();
        var uow = new FakeUnitOfWork();

        var handler = new AddAlbumByBarcodeHandler(discogs, albumRepo, collectionRepo, uow);

        // Act
        var result = await handler.HandleAsync(new AddAlbumByBarcodeCommand(barcode));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(discogsId, result!.DiscogsReleaseId);
        Assert.Single(collectionRepo.Items);
        Assert.Single(albumRepo.Albums);
    }

    private class FakeDiscogsClient(long id) : IDiscogsClient
    {
        public Task<DiscogsRelease?> GetReleaseAsync(long releaseId, CancellationToken cancellationToken = default)
            => Task.FromResult<DiscogsRelease?>(new DiscogsRelease(id, "Album Title", "Artist", 1999, "US", null, ["1234567890123"]));

        public Task<DiscogsSearchResponse?> SearchByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
            => Task.FromResult<DiscogsSearchResponse?>(new DiscogsSearchResponse([new DiscogsSearchResult(id, "Album Title", null, 1999, "US", [barcode], null, null)]));
    }

    private class InMemoryAlbumRepository : IAlbumRepository
    {
        public List<Album> Albums { get; } = [];
        public Task AddAsync(Album album, CancellationToken cancellationToken = default)
        {
            Albums.Add(album);
            return Task.CompletedTask;
        }
        public Task<Album?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
            => Task.FromResult(Albums.FirstOrDefault(a => a.Barcode == barcode));
        public Task<Album?> GetByDiscogsIdAsync(long discogsReleaseId, CancellationToken cancellationToken = default)
            => Task.FromResult(Albums.FirstOrDefault(a => a.DiscogsReleaseId == discogsReleaseId));
    }

    private class InMemoryCollectionRepository : ICollectionRepository
    {
        public List<CollectionItem> Items { get; } = [];
        public Task AddAsync(CollectionItem item, CancellationToken cancellationToken = default)
        {
            Items.Add(item);
            return Task.CompletedTask;
        }
    }

    private class FakeUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
    }
}
