using System.Net.Http.Json;

namespace MusicMan.Mobile.Services;

public sealed class CollectionApiClient
{
    private readonly HttpClient _http;

    // Typed client pattern: DI will provide a preconfigured HttpClient
    public CollectionApiClient(HttpClient httpClient)
    {
        _http = httpClient;
    }

    public async Task<bool> AddByBarcodeAsync(string barcode, CancellationToken ct = default)
    {
        var request = new AddByBarcodeRequest { Barcode = barcode };
        using var resp = await _http.PostAsJsonAsync("/api/collection/barcodes", request, ct);
        return resp.IsSuccessStatusCode;
    }

    private sealed class AddByBarcodeRequest
    {
        public string Barcode { get; set; } = string.Empty;
    }
}
