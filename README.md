# MusicMan

An app for managing your music collection. This solution uses .NET 9, Aspire AppHost, Minimal APIs, and a DDD/Clean Architecture layout. Development persistence uses EF Core with SQLite.

## Prerequisites

- .NET SDK 9 (Preview acceptable; see `global.json` if present)
- PowerShell (Windows) or a shell of your choice
- Optional (for orchestration): Aspire workloads installed by the .NET SDK templates

## Solution layout (Clean Architecture)

- `src/MusicMan.Domain` — domain model (aggregates, entities, value objects, events)
- `src/MusicMan.Application` — use cases, abstractions, behaviors, validation
- `src/MusicMan.Infrastructure` — EF Core, repositories, integrations, DI wiring
  - `Persistence/AppDbContext.cs`
  - `DependencyInjection/ServiceCollectionExtensions.cs`
  - `Persistence/Migrations/` (EF migrations live here)
- `src/MusicMan.WebApi` — thin Minimal API endpoints; delegates to Application layer
  - `Endpoints/WeatherForecast/WeatherForecastEndpoint.cs` → `GET /weatherforecast`
  - `Endpoints/Collection/AddByBarcodeEndpoint.cs` → `POST /api/collection/barcodes`
- `src/MusicMan.ServiceDefaults` — shared conventions (OpenTelemetry, health, resilient HttpClient)
- `src/MusicMan.AppHost` — Aspire AppHost for local orchestration
- `src/MusicMan.Mobile` — MAUI client (calls Web API via HttpClient)
- Tests: `tests/MusicMan.Domain.Tests`, `tests/MusicMan.Application.Tests`, `tests/MusicMan.Architecture.Tests`

## Getting started

From repo root:

```powershell
# Restore/build
dotnet build MusicMan.sln

# Run tests
dotnet test MusicMan.sln
```

### Run the Web API (Development)

```powershell
dotnet run --project .\src\MusicMan.WebApi
```

Development URLs (from launchSettings):

- Web API: <http://localhost:5031> and <https://localhost:7266>
- Swagger UI: <http://localhost:5031/swagger/index.html>
- OpenAPI JSON: <http://localhost:5031/openapi/v1.json>
- Health: <http://localhost:5031/health> (readiness), <http://localhost:5031/alive> (liveness) — Development only

### Run with Aspire AppHost (recommended)

```powershell
dotnet run --project .\src\MusicMan.AppHost\MusicMan.AppHost.csproj
```

AppHost dev URLs (see `src/MusicMan.AppHost/Properties/launchSettings.json`):

- HTTP: <http://localhost:15166>
- HTTPS: <https://localhost:17072>

## EF Core + SQLite (development)

- Connection string: `ConnectionStrings:DefaultConnection` in `src/MusicMan.WebApi/appsettings.Development.json` (defaults to `Data Source=musicman_dev.db`).
- On app start (Development), automatic migrations are applied via `db.Database.Migrate()`.
- A local dotnet tool manifest is included for EF tooling.

Add a migration (run from repo root):

```powershell
dotnet tool run dotnet-ef migrations add InitialCreate -p .\src\MusicMan.Infrastructure -s .\src\MusicMan.WebApi -o Persistence\Migrations
```

Update the database (optional; startup also migrates in Development):

```powershell
dotnet tool run dotnet-ef database update -p .\src\MusicMan.Infrastructure -s .\src\MusicMan.WebApi
```

## Patterns and conventions

- Endpoints are extension methods on `IEndpointRouteBuilder` with co-located request/response types.
- Always include service defaults in Web API: `builder.AddServiceDefaults(); app.MapDefaultEndpoints();`.
- Infrastructure is registered in Web with `builder.Services.AddInfrastructure(builder.Configuration)` to add EF Core and DB health checks.
- Prefer DI `HttpClient`; ServiceDefaults adds resilience and service discovery.

## Mobile client (dev)

- `src/MusicMan.Mobile/Services/CollectionApiClient.cs` calls the Web API.
- Base URL defaults to `http://localhost:5031`; Android emulator uses `http://10.0.2.2:5031`.

## Troubleshooting

- Build errors about missing EF packages: ensure the restore completed; the solution includes EF packages in Infrastructure.
- Migrations not applying: check you’re in Development (`ASPNETCORE_ENVIRONMENT=Development`) or run `dotnet-ef database update`.
- Swagger UI not visible: ensure running in Development and navigate to `/swagger/index.html`.
- SQLite file location: `musicman_dev.db` is created in the working directory of the running process.

## For AI agents

See `.github/copilot-instructions.md` for concise conventions, endpoints pattern, EF/SQLite specifics, dev URLs, and example file references.
