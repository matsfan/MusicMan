# MusicMan Domain Design

This document captures a pragmatic, MVP-first domain model for MusicMan, aligned with the current code and leaving room for growth. It favors simple types today and introduces value objects and additional entities when they clearly add value.

## Bounded Contexts (initial)

- Catalog (reference data)
  - Purpose: canonical metadata about releases (sourced primarily from Discogs for now).
  - Aggregate root: Album.
- Collection (ownership)
  - Purpose: a user’s owned items referencing entries from Catalog.
  - Aggregate root: CollectionItem (referencing Album by Id).

Note: There is no explicit User/Collection aggregate yet. If multi-user scenarios arise, we’ll introduce `Collection (User) -> CollectionItems` as a parent aggregate.

## Aggregates and Entities

### Album (Catalog AR)

Represents metadata for a specific Discogs release.

• Identity: `Id : Guid`

• External identity: `DiscogsReleaseId : long` (required, > 0)

• Core metadata: `Title : string` (required, non-empty), `Artist : string` (required, non-empty)

• Optional metadata: `Year : int?`, `Country : string?`, `CoverImageUrl : string?`

• Barcodes: `Barcode : string?` (MVP) → future: promote to `IReadOnlyCollection<Barcode>` value objects

Invariants (MVP):

- `DiscogsReleaseId > 0`
- `Title` and `Artist` not null/whitespace
- `Year` if present is within a reasonable range (e.g., 1880..currentYear+1) – can be enforced later

Notes:

- We model a specific release (Discogs “release”, not “master”) to match the barcode-based lookup flow.
- We keep `Artist` as a simple string for now. If we need richer graph (multiple artists, roles), we can promote it.

### CollectionItem (Collection AR)

Represents ownership of an Album in the local collection.

- Identity: `Id : Guid`
- Relationship: `AlbumId : Guid` (FK), `Album : Album`
- Ownership metadata: `AddedAtUtc : DateTime` (set on creation), `Notes : string?`

MVP invariants:

- Must reference a valid `Album`

Future extensions (non-breaking):

- Format: `Format : ReleaseFormat` (enum: Vinyl, CD, Digital, Cassette, …)
- Condition: `MediaCondition`, `SleeveCondition` (enums or value objects)
- Purchase info: `Purchase : PurchaseInfo` (VO: price, currency, date, source)
- Location: `StorageLocation` (VO)

## Value Objects (future-friendly)

Introduce these incrementally to add correctness without bloating MVP:

- `Barcode`
  - Normalized string, EAN/UPC validation, equality by normalized value.
- `CountryCode`
  - ISO 3166-1 alpha-2, optional
- `Year`
  - Validated int within acceptable range
- `DiscogsId`
  - Wraps `long` (>0), reduces primitive obsession
- `ReleaseFormat`, `Condition`
  - Enums or VOs depending on needs

## Domain Events (optional)

- `AlbumAddedToCollection` (CollectionItem created)
  - Useful for notifications, projections, or integrations later.

## Repositories and Units of Work

- `IAlbumRepository`
  - `GetByDiscogsIdAsync(long)`, `GetByBarcodeAsync(string)`, `AddAsync(Album)`
- `ICollectionRepository`
  - `AddAsync(CollectionItem)` (and later: querying/paging)
- `IUnitOfWork`
  - `SaveChangesAsync()`

These are already reflected in Application layer abstractions and tests. Infrastructure uses EF Core (SQLite in dev) and should enforce keys, FKs, and indexes.

## Persistence and EF Core

- Keys: `Album.Id`, `CollectionItem.Id`
- Relationships: `CollectionItem.AlbumId` → `Album.Id` (required)
- Indexes (recommended):
  - `Album.DiscogsReleaseId` (unique)
  - `Album.Barcode` (non-unique for now; can be unique when we manage multi-barcode properly)
- Migrations: auto-run on startup in Development per existing `Program.cs`

## Mapping from Discogs (ACL)

Anti-corruption layer exists via `IDiscogsClient`. The mapping from `DiscogsRelease` → `Album` should:

- Normalize `Title`, `Artists`, `Year`, `Country`, `CoverImageUrl`
- Prefer the request barcode when available; later, capture all barcodes as a collection

## Folder conventions

Keep it simple at MVP and evolve organization when the model grows.

- Start in `Entities/` when the aggregate is small (root + a couple of simple members) and behavior is light.
- Move to `Aggregates/<Name>/` when you add multiple entities/VOs tightly scoped to the root, richer invariants, or specs.
- Keep value objects in `ValueObjects/` unless they are very specific to a single aggregate (then colocate under that aggregate folder).
- Align namespaces with folders for clarity (e.g., `MusicMan.Domain.Aggregates.Album`).

Example (later, when it grows):

```text
src/MusicMan.Domain/
  Aggregates/
    Album/
      Album.cs
      AlbumTrack.cs
      Specifications/
    Collection/
      CollectionItem.cs
  ValueObjects/
    Barcode.cs
```

For now, leaving `Album` and `CollectionItem` under `Entities/` is fine. We’ll promote to `Aggregates/` when we introduce more behavior (e.g., multiple barcodes, purchase/condition VOs, etc.).

## Incremental Evolution Plan (safe steps)

1. Strengthen invariants (no breaking API)

- Add guard clauses in `Album` and `CollectionItem` constructors (non-empty Title/Artist, positive DiscogsReleaseId, non-null Album)
- Add minimal unit tests

1. EF Core configuration and indexes

- Add EF configuration classes for `Album` and `CollectionItem`
- Create migration: unique index on `Album.DiscogsReleaseId`, index on `Album.Barcode`

1. Introduce `Barcode` value object (compat mode)

- Internally store normalized barcode; keep external API accepting `string` but map to VO in Application layer
- Backfill any existing data

1. Ownership enrichment (opt-in)

- Add optional fields for Format/Condition/Purchase/Location on `CollectionItem`
- Keep defaults to preserve current behavior

1. Multi-user collections (if/when needed)

- Introduce `User` and `Collection` aggregate owning `CollectionItems`
- Add a `UserId` to `CollectionItem` and tighten repository queries

## Open Questions

- Do we need to support multiple barcodes per release now? (Discogs data often contains many.)
- Are we targeting multi-user from day one, or is this a single local collection?
- How rich should artist/credit modeling be (e.g., multiple artists, roles, featuring)?

---

If this looks good, we can start with step (1): guard clauses + tests, then add EF configuration and indexes.
