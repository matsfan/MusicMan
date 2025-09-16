# Copilot instructions for MusicMan

Purpose: Make AI agents instantly productive in this .NET 9 + Aspire solution using Minimal APIs and a Clean/DDD layout. SQLite via EF Core in Development.

Architecture & flow

- Web API (`src/MusicMan.WebApi`): Minimal API host. `Program.cs` calls `builder.AddServiceDefaults()`, adds OpenAPI/Swagger, wires Infrastructure (`AddInfrastructure`) and Application (`AddApplication`), maps health (`app.MapDefaultEndpoints()`), then feature endpoints (e.g., `app.MapAddByBarcode(); app.MapWeatherForecast();`).
- Application (`src/MusicMan.Application`): Use cases + ports. Example: `UseCases/Collection/AddAlbumByBarcode.cs` defines `AddAlbumByBarcodeCommand`, handler interface/impl, and result record. Web should delegate here.
- Domain (`src/MusicMan.Domain`): Entities only (e.g., `Album`, `CollectionItem`). No EF or web concerns.
- Infrastructure (`src/MusicMan.Infrastructure`): EF Core + integrations. `Persistence/AppDbContext` implements `IUnitOfWork`; DI registers repositories (`IAlbumRepository`, `ICollectionRepository`) and Discogs client; DB health check.
- Service Defaults (`src/MusicMan.ServiceDefaults`): OpenTelemetry, resilient HttpClient, service discovery, and Development-only health endpoints `/health`, `/alive`.
- Aspire AppHost (`src/MusicMan.AppHost`): Local orchestration (`builder.AddProject<Projects.MusicMan_WebApi>("musicman-web")`).

Endpoint conventions

- Endpoints are extension methods on `IEndpointRouteBuilder` with co-located request/response records. Example: `Endpoints/Collection/AddByBarcodeEndpoint.cs` maps `POST /api/collection/barcodes`, accepts `AddByBarcodeRequest`, injects `IAddAlbumByBarcodeHandler`, returns typed `AddByBarcodeResponse`, and configures Swagger `Accepts/Produces` and examples.
- Map all endpoints explicitly in `Program.cs`. Keep business logic out of Web; call Application handlers.

DI and external services

- `AddInfrastructure` (Infrastructure): registers `DbContext` (SQLite), repositories, `IUnitOfWork` via `AppDbContext`, and `IDiscogsClient` with typed `HttpClient` (configure via `Discogs` section; prefer env var `Discogs__Token`).
- `AddApplication` (Application): registers use case handlers (e.g., `IAddAlbumByBarcodeHandler`).

EF Core & database

- Connection: `ConnectionStrings:DefaultConnection` (default `Data Source=musicman_dev.db`) in `src/MusicMan.WebApi/appsettings.Development.json`.
- Migrations live in `src/MusicMan.Infrastructure/Persistence/Migrations/`; `DesignTimeDbContextFactory` enables `dotnet-ef`.
- In Development, `Program.cs` runs `db.Database.Migrate()` at startup.

Developer workflows

- Build/test (repo root): `dotnet build MusicMan.sln`; `dotnet test MusicMan.sln` (VS Code task: "test-solution").
- Run Web API (Dev): `dotnet run --project src/MusicMan.WebApi` → HTTP http://localhost:5031, HTTPS https://localhost:7266; Swagger `/swagger/index.html`; OpenAPI `/openapi/v1.json`.
- Health (Dev): `/health` (readiness incl. DB), `/alive` (liveness).
- AppHost (recommended): `dotnet run --project src/MusicMan.AppHost` (see `Properties/launchSettings.json` for ports).

Cross-app integration

- Mobile (`src/MusicMan.Mobile/Services/CollectionApiClient.cs`) calls Web API via `HttpClient`. Dev base `http://localhost:5031` (Android emulator: `http://10.0.2.2:5031`).

Files to know

- Web composition: `src/MusicMan.WebApi/Program.cs`
- Use case example: `src/MusicMan.Application/UseCases/Collection/AddAlbumByBarcode.cs`
- EF + DI: `src/MusicMan.Infrastructure/Persistence/AppDbContext.cs`, `DependencyInjection/ServiceCollectionExtensions.cs`, `External/Discogs/DiscogsClient.cs`
- Service defaults: `src/MusicMan.ServiceDefaults/Extensions.cs`; AppHost: `src/MusicMan.AppHost/AppHost.cs`

When adding features, follow the endpoint pattern above, register handlers in Application, repositories in Infrastructure, and map endpoints in `Program.cs`. If any of the above diverges (new services/endpoints, ports, DB), update this file.
