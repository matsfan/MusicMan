using MusicMan.Domain.Entities;

namespace MusicMan.Domain.Tests;

public class AlbumTests
{
    [Fact]
    public void Ctor_ValidInputs_CreatesAlbum()
    {
        var album = new Album(42, "Title", "Artist", 1999, "US", null, "1234567890123");
        Assert.Equal(42, album.DiscogsReleaseId);
        Assert.Equal("Title", album.Title);
        Assert.Equal("Artist", album.Artist);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Ctor_InvalidDiscogsId_Throws(long invalidId)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Album(invalidId, "T", "A", null, null, null, null));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Ctor_InvalidTitle_Throws(string? title)
    {
        Assert.Throws<ArgumentException>(() => new Album(1, title!, "A", null, null, null, null));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Ctor_InvalidArtist_Throws(string? artist)
    {
        Assert.Throws<ArgumentException>(() => new Album(1, "T", artist!, null, null, null, null));
    }
}
