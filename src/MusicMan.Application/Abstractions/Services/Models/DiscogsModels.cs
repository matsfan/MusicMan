namespace MusicMan.Application.Abstractions.Services.Models;

public record DiscogsSearchResponse(List<DiscogsSearchResult> Results);
public record DiscogsSearchResult(long Id, string Title, string? CoverImage, int? Year, string? Country, List<string>? Barcode, List<string>? Genre, List<string>? Style);

public record DiscogsRelease(
    long Id,
    string Title,
    string Artists,
    int? Year,
    string? Country,
    string? CoverImageUrl,
    List<string>? Barcodes
);
