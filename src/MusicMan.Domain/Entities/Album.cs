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
        if (discogsReleaseId <= 0)
            throw new ArgumentOutOfRangeException(nameof(discogsReleaseId), "Discogs release id must be positive.");
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(artist))
            throw new ArgumentException("Artist is required.", nameof(artist));

        DiscogsReleaseId = discogsReleaseId;
        Title = title.Trim();
        Artist = artist.Trim();
        Year = year;
        Country = country;
        CoverImageUrl = coverImageUrl;
        Barcode = barcode;
    }
}
