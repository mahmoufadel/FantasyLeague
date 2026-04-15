---
description: Scaffold a complete new domain entity with all layers (domain, application, infrastructure, controller, tests)
context: fork
allowed-tools:
  - Read
  - Write
  - Edit
  - Glob
  - Grep
  - Bash
---

# Skill: add-entity

Scaffold a complete new entity across all layers. The argument is the entity name in PascalCase (e.g. `Transfer`).

Usage: `/add-entity Transfer`

## Step 0 — Read before writing

Before creating any file:
1. Read `src/Domain/Entities/Player.cs` — use it as the canonical domain entity template
2. Read `src/Application/Services/PlayerService.cs` — use it as the canonical service template
3. Read `src/Infrastructure/Repositories/PlayerRepository.cs` — use it as the canonical repository template
4. Read `src/WebApi/Controllers/PlayersController.cs` — use it as the canonical controller template
5. Read `tests/UnitTests/PlayerServiceTests.cs` — use it as the canonical service test template
6. Read `tests/UnitTests/GameWeekTests.cs` — use it as the canonical domain entity test template
7. Read `src/Application/DTOs/Dtos.cs` to understand existing DTO patterns
8. Read `src/Application/Interfaces/IRepositories.cs` to understand existing interface patterns

Do NOT skip this step. All generated code must exactly match the style of the files you read.

## Step 1 — Gather entity properties

If the user has listed properties (e.g. `Transfer FromTeamId:Guid ToTeamId:Guid PlayerId:Guid Fee:decimal`), use them.
Otherwise, ask: "What properties should `{EntityName}` have? List as `Name:Type` pairs."

## Step 2 — Create files in this exact order

### 2a. Domain entity
**Path:** `src/Domain/Entities/{EntityName}.cs`

Rules (from `domain` rules file):
- Inherit from `Entity`
- `private set;` on every property
- Private parameterless constructor for EF Core
- Validate every constructor parameter (null checks, range checks)
- Validate every mutating method parameter
- `ArgumentException` / `ArgumentNullException` / `InvalidOperationException` only
- No `async`, no framework imports, no DTO references

### 2b. DTOs
**Path:** `src/Application/DTOs/{EntityName}Dtos.cs`

Add to the existing `Dtos.cs` only if the file is getting large; otherwise append in `Dtos.cs`. Use `record` types. Minimum:
- `Create{EntityName}Dto(...)` — properties that the caller supplies at creation time
- `{EntityName}Dto(Guid Id, ...)` — full read projection

### 2c. Repository interface
**Path:** Append to `src/Application/Interfaces/IRepositories.cs`

Standard members with `CancellationToken ct = default`:
- `GetAllAsync`, `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`

### 2d. Service interface + implementation
**Path:** `src/Application/Services/{EntityName}Service.cs`

- Define `I{EntityName}Service` interface in the same file
- Implement all CRUD methods, returning DTOs
- Include a `private static {EntityName}Dto MapToDto({EntityName} e)` method
- All methods: `async Task<T>`, `CancellationToken cancellationToken = default`

### 2e. Repository implementation
**Path:** `src/Infrastructure/Repositories/{EntityName}Repository.cs`

- Implement `I{EntityName}Repository`
- Constructor-inject `FantasyPremierLeagueDbContext`
- Pass `CancellationToken` to all EF Core calls

### 2f. Controller
**Path:** `src/WebApi/Controllers/{EntityName}sController.cs`

- `[ApiController]`, `[Route("api/[controller]")]`, inherits `ControllerBase`
- Inject `I{EntityName}Service` only — no repositories
- Standard HTTP status codes per `api-controllers` rules

### 2g. Update DbContext
**Path:** `src/Infrastructure/Persistence/FantasyPremierLeagueDbContext.cs`

Add: `public DbSet<{EntityName}> {EntityName}s { get; set; } = null!;`

### 2h. Update DI registration
**Path:** `src/WebApi/Program.cs`

Add two `AddScoped` calls — interface to implementation — for both repository and service.

### 2i. Unit tests — domain entity
**Path:** `tests/UnitTests/{EntityName}Tests.cs`

- `namespace UnitTests.Domain.Entities;`
- Use `[Theory, InlineData]` for data-driven cases, `[Fact]` for single-scenario behaviour tests
- Test: valid construction, each invalid input, state transitions if any
- Naming: `{Method}_When{Condition}_Should{Result}`

### 2j. Unit tests — service
**Path:** `tests/UnitTests/{EntityName}ServiceTests.cs`

- `namespace UnitTests;`
- `[Theory, AutoMockData]` for every test
- Inject `Mock<I{EntityName}Repository>` as parameter — no constructor fields
- Always include `// Arrange`, `// Act`, `// Assert`
- Test: create, getById (found), getById (not found), delete (found), delete (not found)

## Step 3 — Build verification

After all files are created, run:
```bash
cd src && dotnet build ../FantasyPremierLeague.sln --no-restore -v quiet
```

If build fails, read the error output and fix the compilation errors before reporting done.

## Step 4 — Report

Print a summary:

```
=== add-entity: {EntityName} ===

Files created:
  src/Domain/Entities/{EntityName}.cs
  src/Application/DTOs/{EntityName}Dtos.cs           (appended)
  src/Application/Interfaces/IRepositories.cs         (appended)
  src/Application/Services/{EntityName}Service.cs
  src/Infrastructure/Repositories/{EntityName}Repository.cs
  src/WebApi/Controllers/{EntityName}sController.cs
  tests/UnitTests/{EntityName}Tests.cs
  tests/UnitTests/{EntityName}ServiceTests.cs

Files updated:
  src/Infrastructure/Persistence/FantasyPremierLeagueDbContext.cs
  src/WebApi/Program.cs

Build: PASS
```

## Constraints

- Never modify domain entities that already exist
- Never delete any file
- Never touch the React frontend
- Never change test helpers (`AutoMockDataAttribute.cs`)
- If a file already exists at a target path, stop and report the conflict — do not overwrite
