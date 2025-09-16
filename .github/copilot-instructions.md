# Copilot instructions for MusicMan

Purpose: Make AI coding agents productive in this .NET 9 + Aspire solution that follows DDD/Clean Architecture.

## Architecture at a glance

- `src/MusicMan.WebApi` — Minimal API (net9.0). Entrypoint `Program.cs` calls `builder.AddServiceDefaults()`, maps OpenAPI in Development, and `app.MapDefaultEndpoints()` for health. Endpoints use an extension-method pattern per feature.
- `src/MusicMan.AppHost` — Aspire AppHost for local orchestration. `AppHost.cs` wires the web API via `builder.AddProject<Projects.MusicMan_WebApi>("musicman-web")`.
- `src/MusicMan.ServiceDefaults` — `Extensions.cs` provides shared conventions:
  - OpenTelemetry logging/metrics/traces (enable OTLP via `OTEL_EXPORTER_OTLP_ENDPOINT`)
  - Health checks mapped only in Development (`/health`, `/alive`)
  - Service discovery + resilient HttpClient defaults (`AddStandardResilienceHandler` + `AddServiceDiscovery`)
- DDD layers: `MusicMan.Domain` (entities/aggregates), `MusicMan.Application` (use cases/ports), `MusicMan.Infrastructure` (adapters/EF/repos). Web stays thin.

## Endpoint pattern (WebApi)

- Endpoints are extension methods on `IEndpointRouteBuilder` with co-located request/response records:
  - `Endpoints/WeatherForecast/WeatherForecastEndpoint.cs` — `MapWeatherForecast()` defines `GET /weatherforecast` returning an array of `WeatherForecast`.
  - `Endpoints/Collection/AddByBarcodeEndpoint.cs` — `MapAddByBarcode()` maps `POST /api/collection/barcodes` with `AddByBarcodeRequest/Response` (TODO: dispatch to Application layer).
- Endpoints are mapped in `Program.cs` (e.g., `app.MapWeatherForecast(); app.MapAddByBarcode();`). Keep business logic out of Web; delegate to Application handlers/ports.

## Conventions and dependencies

- Always call `builder.AddServiceDefaults()` and `app.MapDefaultEndpoints()` in HTTP services.
- Health endpoints are available only in Development; don’t rely on them in Production.
- Prefer DI `HttpClient`; defaults inherit resilience and service discovery from `ServiceDefaults`.
- Add additional services to AppHost with `builder.AddProject<Projects.YourService>("logical-name")` and reference the project in `MusicMan.AppHost.csproj`.

## Local development workflow

- Build and test from repo root (PowerShell): `dotnet build MusicMan.sln`; `dotnet test MusicMan.sln`.
- Run with Aspire AppHost (recommended): Dev URLs from `src/MusicMan.AppHost/Properties/launchSettings.json` — HTTP http://localhost:15166, HTTPS https://localhost:17072.
- Run Web API alone: `src/MusicMan.WebApi/Properties/launchSettings.json` — HTTP http://localhost:5031, HTTPS https://localhost:7266. OpenAPI is mapped only in Development.

## Files to know

- `src/MusicMan.WebApi/Program.cs` — endpoint composition & minimal pipeline.
- `src/MusicMan.ServiceDefaults/Extensions.cs` — telemetry, health, discovery, HttpClient resilience.
- `src/MusicMan.AppHost/AppHost.cs` — Aspire composition.
- Tests: `tests/MusicMan.Domain.Tests`, `tests/MusicMan.Application.Tests`, `tests/MusicMan.Architecture.Tests` (enforce dependency direction).

If anything above diverges from the current code (e.g., new services/endpoints), tell me what changed and I’ll refine these instructions.

- `src/MusicMan.*/*.csproj` — target frameworks and packages.
- `src/*/appsettings*.json` — logging and env config.

If anything here looks off or is missing (e.g., more services are added), tell me what changed and I’ll refine these instructions.
