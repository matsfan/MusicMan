using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MusicMan.WebApi.Endpoints.Collection;

public static class AddByBarcodeEndpoint
{
    public static IEndpointRouteBuilder MapAddByBarcode(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/collection/barcodes", (AddByBarcodeRequest req) =>
        {
            if (string.IsNullOrWhiteSpace(req.Barcode))
                return Results.BadRequest(new { error = "Barcode is required." });

            // TODO: dispatch to Application layer for real processing
            return Results.Ok(new AddByBarcodeResponse(req.Barcode));
        })
        .WithName("AddByBarcode");

        return app;
    }

    public record AddByBarcodeRequest(string Barcode);
    public record AddByBarcodeResponse(string Received);
}
