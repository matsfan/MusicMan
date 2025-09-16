# Copilot instructions for MusicMan

Purpose: Help AI coding agents be productive in this .NET Aspire solution using a DDD-style layering.

## Architecture at a glance

- `src/MusicMan.Web` — Minimal API (net9.0). Entrypoint `Program.cs`. Adds OpenAPI and maps Aspire default endpoints.
- `src/MusicMan.AppHost` — Aspire AppHost for local orchestration. `AppHost.cs` wires the Web API via `AddProject<Projects.MusicMan_WebApi>("musicman-web")`.
- `src/MusicMan.ServiceDefaults` — Shared conventions via `AddServiceDefaults()`:
  - OpenTelemetry logging/metrics/traces (OTLP toggled by `OTEL_EXPORTER_OTLP_ENDPOINT`)
  - Health checks (`/health`, `/alive` in Development only)
  - Service discovery + resilient HttpClient defaults
- `src/MusicMan.Domain` / `src/MusicMan.Application` / `src/MusicMan.Infrastructure` — DDD layers scaffolded.
- Current endpoint: `/weatherforecast` (sample). Swagger served in Development.

## Conventions and patterns

- Always call `builder.AddServiceDefaults()` and `app.MapDefaultEndpoints()`.
- Health endpoints exist only in Development; don’t rely on them in Production.
- Prefer DI `HttpClient`; resilience + discovery are preconfigured in `Extensions.ConfigureHttpClientDefaults()`.
- Add new services to AppHost: `builder.AddProject<Projects.YourService>("logical-name")` and reference in `MusicMan.AppHost.csproj`.

## Domain-driven design (DDD)

- Layer responsibilities:
  - Domain: Aggregates, value objects, domain services/events, invariants. No framework or outer-layer deps.
  - Application: Use cases (commands/queries + handlers), DTOs, transactions, orchestrates domain. Depends on Domain only.
  - Infrastructure: Adapters (EF Core, repositories, messaging), integration. Depends on Domain (and Application if needed for abstractions).
  - Web: Thin HTTP endpoints; map/validate and delegate to Application. No domain/infra logic.
- Dependency direction: Domain → none; Application → Domain; Infrastructure → Domain; Web → Application. Avoid reverse deps.
- Placement examples:
  - Domain: `Domain/<Context>/<Aggregate>/Artist.cs`, `ValueObjects/Genre.cs`, `Events/TrackAdded.cs`.
  - Application: `Application/<Feature>/{Commands,Queries,Dtos}/AddTrackCommand.cs`, `GetAlbumQuery.cs`.
  - Infrastructure: `Infrastructure/Persistence/{DbContext,Configurations,Repositories}/`.
- Repositories: For Clean Architecture, prefer repository/gateway interfaces in Application (use cases) so Application stays independent of Infrastructure; implement them in Infrastructure and register in Web DI. If an aggregate-centric repository interface truly belongs to the domain model, you may place it in Domain—but keep it free of infra details.
- Transactions: One application handler = one transaction (`SaveChanges` once per handler if using EF Core).

## Clean architecture alignment

- Layer mapping to projects:
  - Entities (core business rules) → `MusicMan.Domain`
  - Use Cases (application business rules) → `MusicMan.Application` (commands/queries/handlers)
  - Interface Adapters (presenters, repositories, mappers, DB) → `MusicMan.Infrastructure` (and simple mappers/presenters in Web as needed)
  - Frameworks & Drivers (UI, HTTP, hosting, telemetry) → `MusicMan.Web`, `MusicMan.AppHost`, `MusicMan.ServiceDefaults`
- Dependency rule: Source code dependencies point inward only (Frameworks → Adapters → Use Cases → Entities). Web references Application; Infrastructure references Domain (and Application for ports). Domain has no outward references.
- Ports & adapters:
  - Define input/output ports in Application (use case interfaces, repo/gateway abstractions, and DTOs).
  - Implement ports in Infrastructure (EF Core repositories, external services). Wire in Web via DI.
  - Keep endpoint mapping and simple request validation in Web; translate HTTP to Application request DTOs and return response DTOs.
- Boundary models:
  - Transport models (HTTP) live in Web; request/response models for use cases live in Application; Domain exposes behavior via aggregates/value objects—not transport concerns.

## Local development

- Run with Aspire AppHost (recommended):
  - `src/MusicMan.AppHost/Properties/launchSettings.json` URLs:
    - HTTP http://localhost:15166, HTTPS https://localhost:17072
    - Aspire dashboard env via `ASPIRE_*` vars
- Run Web API alone:
  - `src/MusicMan.Web/Properties/launchSettings.json` URLs: HTTP http://localhost:5031, HTTPS https://localhost:7266
  - Swagger in Development (`AddOpenApi` + `MapOpenApi`)
- Build/test from repo root (PowerShell): `dotnet build MusicMan.sln`, `dotnet test MusicMan.sln`

## Adding features

- Web endpoint: map in `src/MusicMan.Web/Program.cs` and delegate to an Application handler (command/query). Keep business rules in Domain.
- Health checks: `builder.AddDefaultHealthChecks()` via ServiceDefaults; add specific checks per service with `builder.Services.AddHealthChecks().Add...`.
- Outgoing HTTP: `builder.Services.AddHttpClient<IMyClient, MyClient>()` (inherits resilience + discovery).

## Testing

- Domain tests: `tests/MusicMan.Domain.Tests` for aggregates/value objects/invariants (pure, fast).
- Application tests: `tests/MusicMan.Application.Tests` for handlers with fakes/in-memory repos.
- Architecture tests: `tests/MusicMan.Architecture.Tests` to enforce dependency direction (Web→Application→Domain only).

## Files to know

- `src/MusicMan.Web/Program.cs` — endpoint composition & DI wiring.
- `src/MusicMan.ServiceDefaults/Extensions.cs` — telemetry, health, discovery conventions.
- `src/MusicMan.AppHost/AppHost.cs` — Aspire service composition.
- `src/MusicMan.*/*.csproj` — target frameworks and packages.
- `src/*/appsettings*.json` — logging and env config.

If anything here looks off or is missing (e.g., more services are added), tell me what changed and I’ll refine these instructions.
