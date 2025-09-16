# Copilot instructions for MusicMan

Purpose: Make AI agents productive in this .NET 9 + Aspire solution that follows DDD/Clean Architecture, with minimal API endpoints and EF Core (SQLite) for dev.

## Architecture at a glance

- Web API (`src/MusicMan.WebApi`): Minimal API. `Program.cs` calls `builder.AddServiceDefaults()`, `builder.Services.AddOpenApi()`, wires Infrastructure with `builder.Services.AddInfrastructure(builder.Configuration)`, maps `app.MapDefaultEndpoints()` (health), and maps feature endpoints.
- Infrastructure (`src/MusicMan.Infrastructure`): EF Core with SQLite for development. `Persistence/AppDbContext.cs`; DI in `DependencyInjection/ServiceCollectionExtensions.cs` registers `DbContext` and a DB health check; migrations live under `Persistence/Migrations/`. A design-time factory (`DesignTimeDbContextFactory.cs`) supports `dotnet-ef`.
- Application/Domain (`src/MusicMan.Application`, `src/MusicMan.Domain`): Use cases + domain model. Web stays thin and should delegate to handlers/ports (no business logic in Web).
- Service Defaults (`src/MusicMan.ServiceDefaults`): `Extensions.cs` sets shared conventions: OpenTelemetry, resilient HttpClient, service discovery, and health endpoints (`/health`, `/alive`) in Development only.
- Aspire AppHost (`src/MusicMan.AppHost`): Local orchestration; wires the Web API via `builder.AddProject<Projects.MusicMan_WebApi>("musicman-web")`.

## Endpoint + DI conventions

- Endpoints are extension methods on `IEndpointRouteBuilder` with co-located request/response types:
  - `Endpoints/WeatherForecast/WeatherForecastEndpoint.cs`: `GET /weatherforecast` returning `WeatherForecast[]`.
  - `Endpoints/Collection/AddByBarcodeEndpoint.cs`: `POST /api/collection/barcodes` with `AddByBarcodeRequest/Response`.
- Map endpoints in `Program.cs` (e.g., `app.MapWeatherForecast(); app.MapAddByBarcode();`). Keep controllers/business logic out of Web; call Application layer.
- Always include: `builder.AddServiceDefaults(); app.MapDefaultEndpoints();` in HTTP services.
- Infrastructure wiring: `builder.Services.AddInfrastructure(builder.Configuration)` registers EF Core (SQLite) and health checks.

## Dev workflows (PowerShell)

- Build/test from repo root:
  - `dotnet build MusicMan.sln`
  - `dotnet test MusicMan.sln`
- Run Web API (Development): http://localhost:5031 and https://localhost:7266
  - OpenAPI (Development only): http(s)://localhost:5031/7266/swagger/index.html
- Run with Aspire AppHost (recommended): AppHost dev URLs are in `src/MusicMan.AppHost/Properties/launchSettings.json` (HTTP 15166, HTTPS 17072) and include the Aspire dashboard endpoints.

## EF Core + SQLite (development)

- Connection string: `ConnectionStrings:DefaultConnection` in `src/MusicMan.WebApi/appsettings.Development.json` (defaults to `Data Source=musicman_dev.db`).
- Auto-migration: In Development, `Program.cs` runs `db.Database.Migrate()` at startup.
- Migrations (local tool installed):
  - Add: `dotnet tool run dotnet-ef migrations add <Name> -p .\src\MusicMan.Infrastructure -s .\src\MusicMan.WebApi -o Persistence\Migrations`
  - Update: `dotnet tool run dotnet-ef database update -p .\src\MusicMan.Infrastructure -s .\src\MusicMan.WebApi`

## Common dev URLs

- Web API (Development): http://localhost:5031, https://localhost:7266
- Swagger UI: http://localhost:5031/swagger/index.html (or HTTPS equivalent)
- OpenAPI JSON: http://localhost:5031/openapi/v1.json
- Health: `/health` (readiness), `/alive` (liveness) available only in Development

## Health, telemetry, and HTTP

- Health endpoints (Development only): `/health` (includes DB check) and `/alive` (liveness only).
- OpenTelemetry enabled; set `OTEL_EXPORTER_OTLP_ENDPOINT` to export traces/metrics/logs.
- Prefer DI `HttpClient`; defaults inherit resilience + service discovery from ServiceDefaults.

## Cross-app integration

- Mobile client (`src/MusicMan.Mobile/Services/CollectionApiClient.cs`) uses `HttpClient` to call Web API (dev base: http://localhost:5031; Android emulator uses http://10.0.2.2:5031).

## Files to know

- Web composition: `src/MusicMan.WebApi/Program.cs`
- Service defaults: `src/MusicMan.ServiceDefaults/Extensions.cs`
- Infrastructure EF: `src/MusicMan.Infrastructure/Persistence/*`, `src/MusicMan.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`
- Aspire wiring: `src/MusicMan.AppHost/AppHost.cs`
- Tests: `tests/MusicMan.Domain.Tests`, `tests/MusicMan.Application.Tests`, `tests/MusicMan.Architecture.Tests`

If any of the above diverges (new services/endpoints, different ports, DB choices), update this file. Feedback welcome—tell me what’s unclear or missing and I’ll refine it.
