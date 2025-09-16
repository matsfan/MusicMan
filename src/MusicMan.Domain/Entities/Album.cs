namespace MusicMan.Domain.Entities;

public class Album
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    // External source identifiers
    public long DiscogsReleaseId { get; private set; }

    // Basic metadata
    public string Title { get; private set; } = string.Empty;
    public string Artist { get; private set; } = string.Empty;
    public int? Year { get; private set; }
    public string? Country { get; private set; }
    public string? CoverImageUrl { get; private set; }
    public string? Barcode { get; private set; }

    private Album() { }

    public Album(long discogsReleaseId, string title, string artist, int? year, string? country, string? coverImageUrl, string? barcode)
    {
        DiscogsReleaseId = discogsReleaseId;
        Title = title;
        Artist = artist;
        Year = year;
        Country = country;
        CoverImageUrl = coverImageUrl;
        Barcode = barcode;
    }
}
