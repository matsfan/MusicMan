namespace MusicMan.Domain.Entities;

public class CollectionItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid AlbumId { get; private set; }
    public Album Album { get; private set; } = default!;

    // Ownership metadata
    public DateTime AddedAtUtc { get; private set; } = DateTime.UtcNow;
    public string? Notes { get; private set; }

    private CollectionItem() { }

    public CollectionItem(Album album, string? notes = null)
    {
        Album = album ?? throw new ArgumentNullException(nameof(album));
        AlbumId = album.Id;
        Notes = notes;
        AddedAtUtc = DateTime.UtcNow; // ensure set at construction time
    }
}
