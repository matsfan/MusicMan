using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicMan.Application.Abstractions.Services;
using MusicMan.Application.Abstractions.Services.Models;

namespace MusicMan.Infrastructure.External.Discogs;

public class DiscogsClientOptions
{
    public string BaseUrl { get; set; } = "https://api.discogs.com";
    public string? Token { get; set; }
    public string UserAgent { get; set; } = "MusicMan/1.0";
}

public class DiscogsClient(HttpClient http, IOptions<DiscogsClientOptions> options, ILogger<DiscogsClient> logger) : IDiscogsClient
{
    private readonly HttpClient _http = http;
    private readonly DiscogsClientOptions _options = options.Value;
    private readonly ILogger<DiscogsClient> _logger = logger;

    public async Task<DiscogsSearchResponse?> SearchByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        var url = $"{_options.BaseUrl.TrimEnd('/')}/database/search?barcode={Uri.EscapeDataString(barcode)}&type=release";
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        AddHeaders(req);
        var res = await _http.SendAsync(req, cancellationToken);
        if (!res.IsSuccessStatusCode)
        {
            _logger.LogWarning("Discogs search failed: {Status} - {Reason}", res.StatusCode, res.ReasonPhrase);
            return null;
        }

        var payload = await res.Content.ReadFromJsonAsync<DiscogsSearchApiResponse>(cancellationToken);
        if (payload is null) return null;

        var results = payload.results.Select(r => new DiscogsSearchResult(
            r.id,
            r.title,
            r.cover_image,
            r.year,
            r.country,
            r.barcode,
            r.genre,
            r.style
        )).ToList();
        return new DiscogsSearchResponse(results);
    }

    public async Task<DiscogsRelease?> GetReleaseAsync(long releaseId, CancellationToken cancellationToken = default)
    {
        var url = $"{_options.BaseUrl.TrimEnd('/')}/releases/{releaseId}";
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        AddHeaders(req);
        var res = await _http.SendAsync(req, cancellationToken);
        if (!res.IsSuccessStatusCode)
        {
            _logger.LogWarning("Discogs get release failed: {Status} - {Reason}", res.StatusCode, res.ReasonPhrase);
            return null;
        }

        var payload = await res.Content.ReadFromJsonAsync<DiscogsReleaseApiResponse>(cancellationToken);
        if (payload is null) return null;

        var artists = payload.artists is { Count: > 0 }
            ? string.Join(", ", payload.artists.Select(a => a.name))
            : payload.artists_sort ?? string.Empty;

        return new DiscogsRelease(
            payload.id,
            payload.title ?? string.Empty,
            artists,
            payload.year,
            payload.country,
            payload.images?.FirstOrDefault()?.uri,
            payload.barcodes
        );
    }

    private void AddHeaders(HttpRequestMessage req)
    {
        req.Headers.UserAgent.ParseAdd(_options.UserAgent);
        if (!string.IsNullOrWhiteSpace(_options.Token))
        {
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Discogs", $"token={_options.Token}");
        }
    }

    // Minimal internal models for API shapes
    private record DiscogsSearchApiResponse(List<SearchResult> results);
    private record SearchResult(long id, string title, string? cover_image, int? year, string? country, List<string>? barcode, List<string>? genre, List<string>? style);

    private record DiscogsReleaseApiResponse(
        long id,
        string? title,
        List<Artist>? artists,
        string? artists_sort,
        int? year,
        string? country,
        List<Image>? images,
        List<string>? barcodes
    );

    private record Artist(string name);
    private record Image(string uri);
}
