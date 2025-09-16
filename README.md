# music-man

An app for managing your music collection.

## Clean Architecture layout

This solution follows a DDD/Clean Architecture layering.

- src/MusicMan.Domain — core domain model (no framework deps)
  - Aggregates/
  - Entities/
  - ValueObjects/
  - Events/
  - Services/
  - Enums/
  - Exceptions/
  - Specifications/
- src/MusicMan.Application — use cases (commands/queries + handlers, ports)
  - Abstractions/
    - Persistence/
    - Messaging/
    - Services/
  - Common/
    - Behaviors/
    - Exceptions/
    - Mappings/
    - Validation/
  - Features/
    - WeatherForecast/
      - Commands/
      - Queries/
      - Dtos/
- src/MusicMan.Infrastructure — adapters (EF Core, repos, messaging, etc.)
  - Persistence/
    - Configurations/
    - Migrations/
    - Repositories/
    - Interceptors/
  - Services/
  - Identity/
  - Messaging/
  - External/
  - DependencyInjection/
- src/MusicMan.WebApi — thin HTTP endpoints delegating to Application (REPR)
  - Endpoints/
    - WeatherForecast/ (Endpoint + Request/Response types co-located)
    - Collection/ (Endpoint + Request/Response types co-located)
  - Middlewares/
  - Filters/
  - Extensions/
  - Configuration/

Tests

- tests/MusicMan.Domain.Tests
  - Aggregates/
  - ValueObjects/
- tests/MusicMan.Application.Tests
  - Features/
  - Common/
- tests/MusicMan.Architecture.Tests
  - Rules/

See `.github/copilot-instructions.md` for additional conventions.
