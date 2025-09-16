using MusicMan.Application.Abstractions.Services.Models;

namespace MusicMan.Application.Abstractions.Services;

public interface IDiscogsClient
{
    Task<DiscogsSearchResponse?> SearchByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    Task<DiscogsRelease?> GetReleaseAsync(long releaseId, CancellationToken cancellationToken = default);
}
