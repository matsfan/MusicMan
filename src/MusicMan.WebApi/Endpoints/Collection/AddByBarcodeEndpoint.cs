using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MusicMan.Application.UseCases.Collection;

namespace MusicMan.WebApi.Endpoints.Collection;

public static class AddByBarcodeEndpoint
{
    public static IEndpointRouteBuilder MapAddByBarcode(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/collection/barcodes", async (AddByBarcodeRequest req, IAddAlbumByBarcodeHandler handler, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(req.Barcode))
                return Results.BadRequest(new { error = "Barcode is required." });

            var result = await handler.HandleAsync(new AddAlbumByBarcodeCommand(req.Barcode), ct);
            if (result is null)
            {
                return Results.NotFound(new { error = "No album found for barcode." });
            }

            return Results.Created($"/api/collection/items/{result.CollectionItemId}", new AddByBarcodeResponse(
                result.CollectionItemId,
                result.AlbumId,
                result.DiscogsReleaseId,
                result.Title,
                result.Artist
            ));
        })
        .WithName("AddByBarcode")
        .WithTags("Collection")
        .Accepts<AddByBarcodeRequest>("application/json")
        .Produces<AddByBarcodeResponse>(StatusCodes.Status201Created, "application/json")
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi(operation =>
        {
            operation.Summary = "Add album to collection by barcode";
            operation.Description = "Looks up the album via Discogs and adds it to the local collection.";
            // Example request
            operation.RequestBody = new Microsoft.OpenApi.Models.OpenApiRequestBody
            {
                Content =
                {
                    ["application/json"] = new Microsoft.OpenApi.Models.OpenApiMediaType
                    {
                        Example = new Microsoft.OpenApi.Any.OpenApiObject
                        {
                            ["barcode"] = new Microsoft.OpenApi.Any.OpenApiString("0603497831297")
                        }
                    }
                }
            };
            // Example 201 response
            operation.Responses["201"] = new Microsoft.OpenApi.Models.OpenApiResponse
            {
                Description = "Created",
                Content =
                {
                    ["application/json"] = new Microsoft.OpenApi.Models.OpenApiMediaType
                    {
                        Example = new Microsoft.OpenApi.Any.OpenApiObject
                        {
                            ["collectionItemId"] = new Microsoft.OpenApi.Any.OpenApiString("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
                            ["albumId"] = new Microsoft.OpenApi.Any.OpenApiString("ffffffff-1111-2222-3333-444444444444"),
                            ["discogsReleaseId"] = new Microsoft.OpenApi.Any.OpenApiLong(1234567),
                            ["title"] = new Microsoft.OpenApi.Any.OpenApiString("Greatest Hits"),
                            ["artist"] = new Microsoft.OpenApi.Any.OpenApiString("Some Artist")
                        }
                    }
                }
            };
            return operation;
        });

        return app;
    }

    public record AddByBarcodeRequest(string Barcode);
    public record AddByBarcodeResponse(Guid CollectionItemId, Guid AlbumId, long DiscogsReleaseId, string Title, string Artist);
}
