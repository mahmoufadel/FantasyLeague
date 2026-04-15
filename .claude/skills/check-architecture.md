---
description: Detect layer-boundary violations and forbidden cross-layer dependencies in the codebase
context: fork
allowed-tools:
  - Grep
  - Glob
  - Bash
---

# Skill: check-architecture

Scan the entire codebase for architecture violations. Use `Grep` and `Glob` only — do not read full file contents. Report every violation with file path and line number.

## Allowed dependency direction

```
WebApi  →  Application  →  Domain
               ↑
         Infrastructure
```

## Checks to run

### 1. Domain must not reference Application or Infrastructure
Search in `src/Domain/**/*.cs` for:
- `using Application`
- `using Infrastructure`
- `using Microsoft.EntityFrameworkCore`
- `using Microsoft.AspNetCore`

### 2. Application must not reference Infrastructure or WebApi
Search in `src/Application/**/*.cs` for:
- `using Infrastructure`
- `using WebApi`
- `using Microsoft.EntityFrameworkCore`
- `DbContext`

### 3. Application must not reference DTOs from Infrastructure
Search in `src/Application/**/*.cs` for:
- `using Infrastructure.`

### 4. Controllers must not reference repositories directly
Search in `src/WebApi/Controllers/**/*.cs` for:
- `IPlayerRepository`
- `ITeamRepository`
- `IGameWeekRepository`
- `IMatchResultRepository`
- `DbContext`
- `FantasyPremierLeagueDbContext`

### 5. Controllers must not reference domain entities in return types
Search in `src/WebApi/Controllers/**/*.cs` for return-type patterns:
- `ActionResult<Player>`
- `ActionResult<Team>`
- `ActionResult<GameWeek>`
- `ActionResult<MatchResult>`
- `IEnumerable<Player>`
- `IEnumerable<Team>`

### 6. Domain entities must not be async
Search in `src/Domain/**/*.cs` for:
- `async Task`
- `async ValueTask`

### 7. Tests must not reference Infrastructure directly (unit tests only)
Search in `tests/UnitTests/**/*.cs` for:
- `using Infrastructure`
- `FantasyPremierLeagueDbContext`

## Output format

```
=== check-architecture report ===

[1] Domain → Application/Infrastructure leakage
    PASS — no violations

[2] Application → Infrastructure/WebApi leakage
    PASS — no violations

[3] Application → Infrastructure DTOs
    PASS — no violations

[4] Controllers reference repositories directly
    FAIL — src/WebApi/Controllers/PlayersController.cs:12  using Infrastructure.Repositories;

[5] Controllers expose domain entities
    PASS — no violations

[6] Async methods in Domain
    PASS — no violations

[7] Unit tests reference Infrastructure
    PASS — no violations

=== Summary ===
Checks passed: 6 / 7
Violations found: 1
```

Do not suggest fixes or rewrite code. Report only.
