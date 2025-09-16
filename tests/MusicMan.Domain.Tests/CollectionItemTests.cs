using MusicMan.Domain.Entities;

namespace MusicMan.Domain.Tests;

public class CollectionItemTests
{
    [Fact]
    public void Ctor_ValidAlbum_SetsAlbumAndId()
    {
        var album = new Album(1, "T", "A", null, null, null, null);
        var item = new CollectionItem(album);

        Assert.Equal(album.Id, item.AlbumId);
        Assert.NotEqual(Guid.Empty, item.Id);
        Assert.True((DateTime.UtcNow - item.AddedAtUtc).TotalSeconds < 2);
    }

    [Fact]
    public void Ctor_NullAlbum_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new CollectionItem(null!));
    }
}
