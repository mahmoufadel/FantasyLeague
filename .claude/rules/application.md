---
description: Application service and DTO conventions for the Application layer
globs: ["src/Application/**/*.cs"]
alwaysApply: false
---

# Application Layer Rules

The Application layer orchestrates use cases. It depends on Domain and defines interfaces that Infrastructure implements.

## Service Pattern

- Services accept **repository interfaces** (`IPlayerRepository`, etc.) via constructor injection — never concrete types
- All public methods are `async Task<T>` and accept `CancellationToken cancellationToken = default` as the last parameter
- Pass the `cancellationToken` to every repository call
- Return DTOs, never domain entities

```csharp
public async Task<PlayerDto?> GetPlayerByIdAsync(Guid id, CancellationToken cancellationToken = default)
{
    var player = await _playerRepository.GetByIdAsync(id, cancellationToken);
    return player is null ? null : MapToDto(player);
}
```

## DTO Mapping

- Use a `private static MapToDto(Entity entity)` method inside the service — no AutoMapper
- DTOs are `record` types defined in `Application/DTOs/`
- Never expose domain entities across layer boundaries

## Repository Interfaces

- Defined in `Application/Interfaces/` — the Application layer owns the contract, Infrastructure implements it
- Async signatures with `CancellationToken`: `Task<T> GetByIdAsync(Guid id, CancellationToken ct = default)`
- Standard interface members: `GetAllAsync`, `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`

## Dependency Direction

```
Application → Domain          ✅
Application → Infrastructure  ✗  (never reference concrete repos or DbContext)
```

## Forbidden in Application

- No `using Microsoft.EntityFrameworkCore.*`
- No direct `DbContext` access
- No business rule logic — delegate to domain entity methods
- No `new Player(...)` for creating domain entities from raw data: that is fine in services, but the entity constructor enforces its own invariants
